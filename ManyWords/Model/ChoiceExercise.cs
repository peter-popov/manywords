using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class ChoiceQuestion : INotifyPropertyChanged
    {
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value != text)
                {
                    text = value;
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

    public class ChoiceAnswer : INotifyPropertyChanged
    {
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value != text)
                {
                    text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        public bool IsCorrect { get; set; }

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


    public abstract class ChoiceExercise : Exercise
    {
        public virtual void SubmitAnswer(ChoiceAnswer answer)
        {
        }


        private ChoiceQuestion question;
        public ChoiceQuestion Question 
        {
            get
            {
                return question;
            }
            set
            {
                if (value != question)
                {
                    question = value;
                    NotifyPropertyChanged("Question");
                }
            }
        }

        private ObservableCollection<ChoiceAnswer> answers = new ObservableCollection<ChoiceAnswer>();
        public ObservableCollection<ChoiceAnswer> Answers
        {
            get
            {
                return answers;
            }
            set
            {
                if (value != answers)
                {
                    answers = value;
                    NotifyPropertyChanged("Answers");
                }
            }
        }

        protected Translation selectCorectTranslation(Word w)
        {
            var list = WordsSelector.takeRandom(w.Translations.Where(x => x.Language == App.LanguagesListModel.MotherLanguage.Code));
            return list.FirstOrDefault();
        }
    }
}
