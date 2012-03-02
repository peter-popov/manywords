using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using ManyWords.WordStorage;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using ManyWords.WordStorage;


namespace ManyWords.Model
{

    public class PlaySound : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MessageBox.Show("Imagine I'm playin sound");
        }
    }

    public class WordListItemModel : INotifyPropertyChanged
    {
        private static PlaySound playCmd = new PlaySound();

        public WordListItemModel(Word w)
        {
            this.word = w;
        }

        private Word word;

        public Word Word
        {
            get
            {
                return word;
            }
            set
            {
                //if (value != word)
                {
                    word = value;
                    NotifyPropertyChanged("Spelling");
                    NotifyPropertyChanged("Translation");
                }
            }
        }

        public string Spelling
        {
            get
            {
                return word.Spelling;
            }
            set
            {
                if (value != word.Spelling)
                {
                    word.Spelling = value;
                    NotifyPropertyChanged("Spelling");
                }
            }
        }

        public string Translation
        {
            get
            {
                return word.Translation;
            }
        }

        public ICommand PlaySound
        {
            get { return playCmd; }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class WordsViewModel : INotifyPropertyChanged
    {
        WordStorage.Storage storage = App.WordStorage;

        public WordsViewModel()
        {

        }

        public IEnumerable<WordListItemModel> All
        {
            get
            {
                return from Word w in storage.Words
                       select new WordListItemModel(w);
            }

        }

        public void Remove(WordListItemModel item)
        {

            if (item != null)
            {
                storage.RemoveWord(item.Word);
                NotifyPropertyChanged("All");
            }

        }

        public void UpdateItem(WordListItemModel item)
        {

            Word w = storage.Find(item.Word.WordID);
            if (w != null)
            {
                item.Word = w;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
