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

namespace ManyWords.WordStorage
{

    public class Word
    {
        public string Id;
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
                          select new Word{ Spelling = w.Spelling, Translation = w.Translation };
            }
        }

        public IEnumerable<Word> Find(string word, bool look_in_translation = false)
        {
            return list;
        }

        public Stream GetSpeachAudioStream(Word w)
        {
            return null;
        }

        public void StoreWord(Word newWord)
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
            
            return;
        }

        public void Dispose()
        {
            if (wordsDB != null)
                wordsDB.Dispose();
        }
    }
}
