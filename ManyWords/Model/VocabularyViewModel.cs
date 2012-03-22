using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ManyWords.Translator;
using ManyWords.WordStorage;
using System.Collections.ObjectModel;

namespace ManyWords.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class VocabularyListItemModel : INotifyPropertyChanged
    {
        Vocabulary vocabulary;

        public VocabularyListItemModel(Vocabulary v)
        {
            this.vocabulary = v;            
            v.PropertyChanged += OnSourceVocabularyChanged;
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

        public int Count
        {
            get 
            {
                return vocabulary.Words.Count; 
            }
        }

        public int LearningCount
        {
            get
            {
                return vocabulary.Words.Count(w => w.State == State.Learning);
            }
        }

        public int LearnedCount
        {
            get
            {
                return vocabulary.Words.Count(w => w.State == State.Learned);
            }
        }

        public string Status
        {
            get
            {
                return string.Format("started: {0}/{1}, lerned: {2}", LearningCount, Count, LearnedCount);
            }
        }

        private void OnSourceVocabularyChanged(object sender, PropertyChangedEventArgs args)
        {
            NotifyPropertyChanged("Status");
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

    /// <summary>
    /// 
    /// </summary>
    public class VocabularyViewModel: INotifyPropertyChanged
    {
        WordStorage.Storage storage = App.WordStorage;

        public VocabularyViewModel()
        {            
            Update();
        }

        public void Update()
        {
            All = new ObservableCollection<VocabularyListItemModel>();
            var res = from Vocabulary v in storage.wordsDB.Vocabularies
                      where v.Language == App.LanguagesListModel.StudyLanguage.Code
                      orderby v.IsClosed descending                      
                      orderby v.Description ascending
                      select v;   
            
            foreach( var r in res ) All.Add( new VocabularyListItemModel(r) );
            NotifyPropertyChanged("All");
        }


        public void OnLanguageModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Update();
        }

    
        public ObservableCollection<VocabularyListItemModel> All { get; private set; }

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
