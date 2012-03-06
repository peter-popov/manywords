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
using System.Collections.ObjectModel;
using ManyWords.WordStorage;

namespace ManyWords.Model
{

    public class AnswerItemModel : INotifyPropertyChanged
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

    public class MultichoiceTrainingModel: INotifyPropertyChanged
    {
        public MultichoiceTrainingModel()
        {
            answers = new ObservableCollection<AnswerItemModel>();
            answers.Add(new AnswerItemModel { Text = "Fuck" });
            answers.Add(new AnswerItemModel { Text = "You" });
            answers.Add(new AnswerItemModel { Text = "zzz" });
            answers.Add(new AnswerItemModel { Text = "one more" });

            currentWord = new WordListItemModel(new WordStorage.Word { Spelling = "Fuuuuck" }, null);
        }

        private WordListItemModel currentWord;

        public WordListItemModel Word
        {
            get
            {
                return currentWord;
            }
            set
            {
                if (value != currentWord)
                {
                    currentWord = value;
                    NotifyPropertyChanged("Word");
                }
            }
        }

        private ObservableCollection<AnswerItemModel> answers;

        public ObservableCollection<AnswerItemModel> Answers
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
