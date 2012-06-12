using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Data.Linq;

namespace ManyWords.WordStorage
{

    public class Storage:IDisposable 
    {
        internal WordsDB wordsDB;
       
        public Storage()
        {
            //If it's a very first run copy database to the isolated storage
            var appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!appSettings.Contains("db_copy"))
            {
                System.Diagnostics.Debug.WriteLine("Copying reference database...");
                DateTime from = DateTime.Now;
                MoveReferenceDatabase();
                System.Diagnostics.Debug.WriteLine("Move finished in {0}ms", (DateTime.Now - from).TotalMilliseconds);
                from = DateTime.Now;
                UpdateReferenceDatabase();
                System.Diagnostics.Debug.WriteLine("Update finished in {0}ms", (DateTime.Now - from).TotalMilliseconds);
                
                appSettings.Add("db_copy", "1");
                appSettings.Save();
            }

            // Now open database
            wordsDB = new WordsDB(WordsDB.DBConnectionString);
            if (wordsDB.DatabaseExists() == false)
            {
                //Create the database
                wordsDB.CreateDatabase();
            }
        }        

        public static void MoveReferenceDatabase()
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a stream for the file in the installation folder.
            using (Stream input = Application.GetResourceStream(new Uri("Words.sdf", UriKind.Relative)).Stream)
            {
                // Create a stream for the new file in isolated storage.
                using (IsolatedStorageFileStream output = iso.CreateFile("Words.sdf"))
                {
                    // Initialize the buffer.
                    byte[] readBuffer = new byte[4096];
                    int bytesRead = -1;

                    // Copy the file from the installation folder to isolated storage. 
                    while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        output.Write(readBuffer, 0, bytesRead);
                    }
                }
            }
        }

        public static void UpdateReferenceDatabase()
        {
            using (var db = new WordsDB(WordsDB.DBConnectionString) )
            {
                if (!db.DatabaseExists()) return;
                DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
                if (dbUpdater.DatabaseSchemaVersion == 0)
                {
                    dbUpdater.AddIndex<Word>("WordIndex");
                    dbUpdater.AddIndex<Word>("WordVocabularyIndex");
                    
                    dbUpdater.AddIndex<Translation>("TranslationIndex");
                    dbUpdater.AddIndex<Translation>("TranslationWordIdIndex");
                    dbUpdater.AddIndex<Translation>("TranslationLanguageIndex");                    

                    dbUpdater.DatabaseSchemaVersion = 1;
                    dbUpdater.Execute();
                }

                System.Diagnostics.Debug.WriteLine("Database schema version is {0}", dbUpdater.DatabaseSchemaVersion);                                
            }            
        }
       
        public IEnumerable<Word> Words
        {
            get
            {                
                return from Word w in wordsDB.Words select w;
            }
        }


        public IEnumerable<Translation> Translations
        {
            get
            {
                return from Translation t in wordsDB.Translations select t;
            }
        }

        public Word Find(int id)
        {
            return (from Word w in wordsDB.Words
                    where w.WordID == id
                    select w).FirstOrDefault();
        }

        public Word Find(string spelling)
        {
            return (from Word w in wordsDB.Words
                    where w.Spelling == spelling.ToLower().Trim()
                    select w).FirstOrDefault();
        }

        public Stream GetSpeachAudioStream(Word word)
        {            
            var word_item = Find(word.Spelling);
            if (word_item != null)
                return LoadAudio(word_item.AudioFile);
            else
                return null;
        }

        public void RemoveVocabulary(Vocabulary v)
        {
            foreach (var word in v.Words)
            {
                removeWordImpl(word);
            }
            foreach (var tl in v.TargetLanguages)
            {
                wordsDB.TargetLanguages.DeleteOnSubmit(tl);
            }
            wordsDB.Vocabularies.DeleteOnSubmit(v);
            wordsDB.SubmitChanges();
            App.VocabularyListModel.Update();
        }

        public void RemoveWord(int id)
        {
            removeWordImpl(Find(id) as Word);
            wordsDB.SubmitChanges();
        }

        public void RemoveWord(Word w)
        {
            removeWordImpl(w);
            wordsDB.SubmitChanges();
        }

        private void removeWordImpl(Word word)
        {           
            if (word != null)
            {
                RemoveAudio(word.AudioFile);

                foreach (Translation t in word.Translations)
                    wordsDB.Translations.DeleteOnSubmit(t);
                wordsDB.Words.DeleteOnSubmit(word);
            }
        }

        public void StoreWord(int id, string spelling, IEnumerable<string> translation, Vocabulary vocabulary, Stream audio)
        {
            Word w = Find(id);

            if (w == null)
            {
                StoreWord(spelling, translation, vocabulary, audio);
                return;
            }

            w.Spelling = spelling;


            var motherLanguage = App.LanguagesListModel.MotherLanguage.Code;

            //remove old translations            
            foreach (Translation t in w.Translations)
            {
                if (t.Language == motherLanguage)
                {
                    wordsDB.Translations.DeleteOnSubmit(t);
                }
            }
            
            // add new translations            
            foreach (string s in translation)
            {                
                Translation tr = new Translation { Spelling = s, Word = w, Language = motherLanguage };
                w.Translations.Add(tr);
                wordsDB.Translations.InsertOnSubmit(tr);
            }

            if (vocabulary != null && vocabulary.ID != w.Vocabulary.ID)
            {
                var oldVocabulary = w.Vocabulary;
                oldVocabulary.Words.Remove(w);
                vocabulary.Words.Add(w);
                App.VocabularyListModel.Update();
            }

            wordsDB.SubmitChanges();

            wordsDB.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, w);
            
            if (audio != null)
                SaveAudio(w, audio);

            return;
        }

        public void StoreWord(string spelling, IEnumerable<string> translation, Vocabulary vocabulary, Stream audio)
        {
            Word item = new Word { Spelling = spelling, Added = DateTime.Now };
            item.State = State.New;
            if (vocabulary == null)
            {
                vocabulary = getDefaultVocabulary();
            }
            vocabulary.Words.Add(item);

            wordsDB.Words.InsertOnSubmit(item);
            
            foreach (string s in translation)
            {
                Translation tr = new Translation { Spelling = s, Word = item };
                item.Translations.Add(tr);
                wordsDB.Translations.InsertOnSubmit(tr);
            }
            
            wordsDB.SubmitChanges();
            App.VocabularyListModel.Update();

            if (audio != null )
                SaveAudio(item, audio);

            return;
        }

        public void Dispose()
        {
            if (wordsDB != null)
                wordsDB.Dispose();
        }

        //Seems like wrong place for this function
        private Vocabulary getDefaultVocabulary()
        {
            var studyLanguage = App.LanguagesListModel.StudyLanguage.Code;
            var motherLanguage = App.LanguagesListModel.MotherLanguage.Code;


            var defaultVocabulary = (from VocabularyTargetLanguage v in wordsDB.TargetLanguages
                          where v.Language == motherLanguage && 
                                v.Vocabulary.Language == studyLanguage 
                          orderby v.Vocabulary.IsPreloaded
                          select v.Vocabulary).FirstOrDefault();

            if (defaultVocabulary == null)
            {
                //need to create a new one
                defaultVocabulary = new Vocabulary { Description = "user words", 
                                                     IsPreloaded = false, 
                                                     IsUsed = true, 
                                                     Language = studyLanguage };
                var tl = new VocabularyTargetLanguage { Language = motherLanguage };
                defaultVocabulary.TargetLanguages.Add(tl);

                wordsDB.Vocabularies.InsertOnSubmit(defaultVocabulary);
                wordsDB.TargetLanguages.InsertOnSubmit(tl);                
            }

            return defaultVocabulary;
        }


        private void RemoveAudio(string filename)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a new folder and call it "MyFolder".
            if (myStore.DirectoryExists("Audio") && myStore.FileExists("Audio\\" + filename))
            {
                myStore.DeleteFile("Audio\\" + filename);
            }
        }

        private Stream LoadAudio(string filename)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a new folder and call it "MyFolder".
            if (!myStore.DirectoryExists("Audio")) return null;

            if (!myStore.FileExists("Audio\\" + filename)) return null;

            // Specify the file path and options.
            return new IsolatedStorageFileStream("Audio\\" + filename, FileMode.Open, FileAccess.Read, FileShare.Read, myStore);
        }

        public void SaveAudio(Word w, Stream audio)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a new folder and call it "MyFolder".
            if (!myStore.DirectoryExists("Audio"))
                myStore.CreateDirectory("Audio");

            // Specify the file path and options.
            using (var isoFileStream = new IsolatedStorageFileStream("Audio\\" + w.AudioFile, FileMode.OpenOrCreate, myStore))
            {
                audio.CopyTo(isoFileStream);
            }
        }
    }
}
