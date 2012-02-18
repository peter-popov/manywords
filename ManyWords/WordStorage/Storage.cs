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

    public class Word
    {
        public int Id;
        public string Spelling;
        public string Translation;        
    };


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
                
                //TODO: Why can't I create List in linq select-new?
                return from WordItem w in wordsDB.Words
                          select new Word{ Id=w.WordId, Spelling = w.Spelling, Translation = w.Translation };
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
            var exists = from WordItem w in wordsDB.Words
                         where w.Spelling == word.Spelling
                         select w;

            var word_item = exists.First();

            return LoadAudio(IdToFilename(word_item.WordId));

            return null;
        }

        public void StoreWord(Word newWord, Stream audio)
        {
            var exists = from WordItem w in wordsDB.Words
                         where w.Spelling == newWord.Spelling 
                         select w;

            if (exists.Count<WordItem>() > 0)
            {
                MessageBox.Show("Word already exists in the database");
                return;
            }

            WordItem item = new WordItem { Spelling = newWord.Spelling, Translation = newWord.Translation };            
            wordsDB.Words.InsertOnSubmit(item);
            wordsDB.SubmitChanges();

            if (audio != null )
                SaveAudio(IdToFilename(item.WordId), audio);

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
