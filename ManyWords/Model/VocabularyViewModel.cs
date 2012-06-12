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
    public class VocabularyListItemModel : INotifyPropertyChanged, ICommand
    {
        #region ICommand interface
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parentModel.Fire(this);
        }
        #endregion

        public Vocabulary Vocabulary{ get; private set; }

        public ICommand Train { get { return this; } }

        VocabularyViewModel parentModel;
        public VocabularyListItemModel(Vocabulary v,  VocabularyViewModel model)
        {
            this.Vocabulary = v;   
            parentModel = model;
            v.PropertyChanged += OnSourceVocabularyChanged;
        }
        
        public string Text
        {
            get
            {
                return Vocabulary.Description;
            }
            set
            {
                if (value != Vocabulary.Description)
                {
                    Vocabulary.Description = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        public int Count
        {
            get 
            {
                return Vocabulary.Words.Count; 
            }
        }

        public int LearningCount
        {
            get
            {
                return Vocabulary.Words.Count(w => w.State == State.Learning);
            }
        }

        public int LearnedCount
        {
            get
            {
                return Vocabulary.Words.Count(w => w.State == State.Learned);
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
            User = new ObservableCollection<VocabularyListItemModel>();

            var res = from VocabularyTargetLanguage tl in storage.wordsDB.TargetLanguages
                      where tl.Language == App.LanguagesListModel.MotherLanguage.Code &&
                            tl.Vocabulary.Language == App.LanguagesListModel.StudyLanguage.Code                  
                      select tl.Vocabulary;


            foreach (var v in res)
            {
                var item = new VocabularyListItemModel(v, this);
                All.Add(item);
                if (!v.IsPreloaded) User.Add(item);
            }
            
            NotifyPropertyChanged("All");
            NotifyPropertyChanged("User");        
        }

        public event EventHandler LearnClicked;

        public void Fire(VocabularyListItemModel item)
        {
            if (LearnClicked != null)
                LearnClicked(item, new EventArgs());
        }


        public void OnLanguageModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Update();
        }

    
        public ObservableCollection<VocabularyListItemModel> All { get; private set; }

        public ObservableCollection<VocabularyListItemModel> User { get; private set; }

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
