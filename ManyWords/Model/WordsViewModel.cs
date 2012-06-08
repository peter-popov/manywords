using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ManyWords.Translator;
using ManyWords.WordStorage;
using System.Text.RegularExpressions;

namespace ManyWords.Model
{

    public class PlaySound : ICommand
    {
        TextToSpeech tts;
        Word word;
        public PlaySound(Word w, TextToSpeech tts)
        {
            this.tts = tts;
            this.word = w;
        }

        public event EventHandler CanExecuteChanged;


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            tts.Speak(word);
        }
    }

    public class WordListItemModel : INotifyPropertyChanged
    {
        private PlaySound playCmd;

        public WordListItemModel(Word w, TextToSpeech tts)
        {
            this.word = w;
            w.PropertyChanged += new PropertyChangedEventHandler(w_PropertyChanged);
            this.translation = w.getTranslation(App.LanguagesListModel.MotherLanguage.Code);
            playCmd = new PlaySound(word, tts);
        }

        void w_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Translation")
            {
                this.translation = word.getTranslation(App.LanguagesListModel.MotherLanguage.Code);
                NotifyPropertyChanged("Translation");
            }
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
                word = value;
                NotifyPropertyChanged("Spelling");
                NotifyPropertyChanged("Translation");
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

        private string translation;
        public string Translation
        {
            get
            {
                return translation;
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
        TextToSpeech tts;
        Vocabulary usedVocabulary = null;

        public WordsViewModel(TextToSpeech tts)
        {
            this.tts = tts;
        }

        public WordsViewModel(TextToSpeech tts, Vocabulary vocabulary)
        {
            this.usedVocabulary = vocabulary;
            this.tts = tts;
        }


        string filter = "";
        public void Filter(string filter)
        {
            this.filter = filter;
            NotifyPropertyChanged("All");
            NotifyPropertyChanged("Learning");
            NotifyPropertyChanged("Learned");
        }

        public IEnumerable<WordListItemModel> All
        {
            get
            {
                if (usedVocabulary == null)
                {
                    return query(App.LanguagesListModel.StudyLanguage.Code, filter);
                }
                else
                {
                    return query(usedVocabulary, filter);
                }
            }

        }

        public IEnumerable<WordListItemModel> Learning
        {
            get
            {
                return All.Where(x => x.Word.State == State.Learning);
            }
        }

        public IEnumerable<WordListItemModel> Learned
        {
            get
            {
                return All.Where(x => x.Word.State == State.Learned);
            }
        }


        private IEnumerable<WordListItemModel> query(string language, string filter)
        {
            Regex re = new Regex(@"\b" + filter, RegexOptions.IgnoreCase);
            
            return from Word w in storage.Words
                   where w.Vocabulary.Language == language
                       && re.IsMatch(w.Spelling)
                   orderby w.Spelling ascending
                   select new WordListItemModel(w, tts);
        }

        private IEnumerable<WordListItemModel> query(Vocabulary vocab, string filter)
        {           
            Regex re = new Regex( @"\b" + filter, RegexOptions.IgnoreCase );
            return from Word w in storage.Words
                   where w.Vocabulary.ID == vocab.ID
                      && re.IsMatch(w.Spelling)
                   orderby w.Spelling ascending
                   select new WordListItemModel(w, tts);
        }


        public void Remove(WordListItemModel item)
        {
            if (item != null)
            {
                storage.RemoveWord(item.Word);
                NotifyPropertyChanged("All");
                NotifyPropertyChanged("Learning");
                NotifyPropertyChanged("Learned");
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
