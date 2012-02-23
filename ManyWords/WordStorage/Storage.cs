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

        private List<Word> list = new List<Word>();


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

        public IEnumerable<Word> Find(string word, bool look_in_translation = false)
        {
            return list;
        }


        string IdToFilename(Int64 id)
        {
            return id.ToString().Trim() + ".wav";
        }

        public Stream GetSpeachAudioStream(Word word)
        {
            var exists = from Word w in wordsDB.Words
                         where w.Spelling == word.Spelling
                         select w;

            var word_item = exists.First();

            return LoadAudio(IdToFilename(word_item.WordID));
        }

        public void StoreWord(string spelling, string[] translation, Stream audio)
        {
            var exists = from Word w in wordsDB.Words
                         where w.Spelling == spelling 
                         select w;

            if (exists.Count<Word>() > 0)
            {
                MessageBox.Show("Word already exists in the database");
                return;
            }

            Word item = new Word { Spelling = spelling };
            //wordsDB.Words.InsertOnSubmit(item);
            //wordsDB.SubmitChanges();
            
            foreach (string s in translation)
            {
                if (s.Trim() == "") continue;
                Translation tr = new Translation { Spelling = s, Word = item };
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
