using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ManyWords.Translator;
using ManyWords.WordStorage;

namespace ManyWords.Model
{

    public class VocabularyListItemModel : INotifyPropertyChanged
    {
        Vocabulary vocabulary;

        public VocabularyListItemModel(Vocabulary v)
        {
            this.vocabulary = v;
        }

        public string Text
        {
            get
            {
                return vocabulary.Description;
            }
            set
            {
                if (value != vocabulary.Description)
                {
                    vocabulary.Description = value;
                    NotifyPropertyChanged("Text");
                }
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


    public class VocabularyViewModel: INotifyPropertyChanged
    {
        WordStorage.Storage storage = App.WordStorage;

        public IEnumerable<VocabularyListItemModel> All
        {
            get
            {
                return from Vocabulary v in storage.wordsDB.Vocabularies
                       orderby v.Description ascending
                       select new VocabularyListItemModel(v);
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
