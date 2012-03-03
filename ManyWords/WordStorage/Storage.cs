using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Linq;
using System.IO.IsolatedStorage;

namespace ManyWords.WordStorage
{

    public class Storage:IDisposable 
    {
        WordsDB wordsDB;

        public Storage()
        {
            // Create the database if it does not exist.
            wordsDB = new WordsDB(WordsDB.DBConnectionString);
            if (wordsDB.DatabaseExists() == false)
            {
                //Create the database
                wordsDB.CreateDatabase();
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

            wordsDB.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, w);
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

            Word item = new Word { Spelling = spelling };
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
