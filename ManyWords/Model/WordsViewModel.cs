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

namespace ManyWords.Model
{

    public class WordListItemModel : INotifyPropertyChanged
    {
        public WordListItemModel(Word w)
        {
            this.word = w;
        }

        private Word word;

        public Word Word
        {
            get { return word; }
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
            set
            {
                if (value != word.Translation)
                {
                    word.Translation = value;
                    NotifyPropertyChanged("Translations");
                }
            }
        }

        void Play(object sender, RoutedEventArgs args)
        {
            MessageBox.Show("Play " + Spelling);
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


    public class WordsViewModel: INotifyPropertyChanged
    {
        WordStorage.Storage storage = new WordStorage.Storage();

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
