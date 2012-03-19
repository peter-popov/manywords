using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ManyWords.Translator;
using ManyWords.WordStorage;

namespace ManyWords.Model
{

    public class PlaySound : ICommand
    {
        TextToSpeech tts;
        WordListItemModel item;
        public PlaySound(WordListItemModel item, TextToSpeech tts)
        {
            this.tts = tts;
            this.item = item;
        }

        public event EventHandler CanExecuteChanged;

        
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            tts.Speak(item.Word);
        }
    }

    public class WordListItemModel : INotifyPropertyChanged
    {
        private PlaySound playCmd;

        public WordListItemModel(Word w, TextToSpeech tts)
        {
            this.word = w;
            playCmd = new PlaySound(this, tts);
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
        TextToSpeech tts;

        public WordsViewModel()
        {
            tts = new TextToSpeech(new Language { Code = "de", Name = "German" },
                                                Translator.TranslatorFactory.CreateInstance());
        }

        public IEnumerable<WordListItemModel> All
        {
            get
            {
                return from Word w in storage.Words
                       orderby w.Spelling ascending
                       select new WordListItemModel(w, tts);
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
