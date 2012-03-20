using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;

namespace ManyWords.WordStorage
{

    public class Storage:IDisposable 
    {
        internal WordsDB wordsDB;
        private Vocabulary defaultVocabulary = null;

        public Storage()
        {
            //If it's a very first run copy database to the isolated storage
            /*var appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (!appSettings.Contains("db_copy"))
            {
                System.Diagnostics.Debug.WriteLine("Copying reference database...");
                DateTime from = DateTime.Now;
                MoveReferenceDatabase();
                System.Diagnostics.Debug.WriteLine("Finished in {0}ms", (DateTime.Now - from).TotalMilliseconds);
                
                appSettings.Add("db_copy", "1");
                appSettings.Save();
            }*/

            // Now open database
            wordsDB = new WordsDB(WordsDB.DBConnectionString);
            if (wordsDB.DatabaseExists() == false)
            {
                //Create the database
                wordsDB.CreateDatabase();
            }

            //
            // Hack!!!
            // To be removed, create default vocabulary
            defaultVocabulary = (from Vocabulary v in wordsDB.Vocabularies
              where v.IsDefault
              select v).FirstOrDefault();
            if (defaultVocabulary == null)
            {
                // If not found need to create
                defaultVocabulary = new Vocabulary { Description = "Default", IsDefault = true, FromLanguage = "de", ToLanguage = "en" };

                wordsDB.Vocabularies.InsertOnSubmit(defaultVocabulary);
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
                    where w.Spelling.ToLower().Trim() == spelling.ToLower().Trim()
                    select w).FirstOrDefault();
        }


        string IdToFilename(Int64 id)
        {
            return id.ToString().Trim() + ".wav";
        }

        public Stream GetSpeachAudioStream(Word word)
        {            
            var word_item = Find(word.Spelling);
            if (word_item != null)
                return LoadAudio(IdToFilename(word_item.WordID));
            else
                return null;
        }

        public void RemoveWord(int id)
        {
            Word word = Find( id);
            if (word != null)
            {
                foreach (Translation t in word.Translations)
                    wordsDB.Translations.DeleteOnSubmit(t);
                wordsDB.Words.DeleteOnSubmit(word);
                wordsDB.SubmitChanges();
            }
        }

        public void RemoveWord(Word w)
        {
            RemoveWord(w.WordID);
        }

        public void StoreWord(int id, string spelling, IEnumerable<string> translation, Stream audio)
        {
            Word w = Find(id);

            if (w == null)
            {
                StoreWord(spelling, translation, audio);
                return;
            }

            w.Spelling = spelling;

            //remove old translations
            foreach (Translation t in w.Translations)
            {
                wordsDB.Translations.DeleteOnSubmit(t);                
            }
            w.Translations.Clear();

            // add new translations            
            foreach (string s in translation)
            {                
                Translation tr = new Translation { Spelling = s, Word = w };
                w.Translations.Add(tr);
                wordsDB.Translations.InsertOnSubmit(tr);
            }

            wordsDB.SubmitChanges();
            
            if (audio != null)
                SaveAudio(IdToFilename(w.WordID), audio);

            return;
        }

        public void StoreWord(string spelling, IEnumerable<string> translation, Stream audio)
        {
            if (Find(spelling) != null)
            {
                MessageBox.Show("Word already exists in the database");
                return;
            }

            Word item = new Word { Spelling = spelling, Added = DateTime.Now };
            item.State = State.New;
            defaultVocabulary.Words.Add(item);

            wordsDB.Words.InsertOnSubmit(item);
            
            foreach (string s in translation)
            {
                Translation tr = new Translation { Spelling = s, Word = item };
                item.Translations.Add(tr);
                wordsDB.Translations.InsertOnSubmit(tr);
            }
            
            wordsDB.SubmitChanges();

            if (audio != null )
                SaveAudio(IdToFilename(item.WordID), audio);

            return;
        }

        public void Dispose()
        {
            if (wordsDB != null)
                wordsDB.Dispose();
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

        private void SaveAudio(string filename, Stream audio)
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a new folder and call it "MyFolder".
            if (!myStore.DirectoryExists("Audio"))
                myStore.CreateDirectory("Audio");

            // Specify the file path and options.
            using (var isoFileStream = new IsolatedStorageFileStream("Audio\\" + filename, FileMode.OpenOrCreate, myStore))
            {
                audio.CopyTo(isoFileStream);
            }
        }
    }
}
