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
    public class ChoiceCorrectTrainingModel: INotifyPropertyChanged
    {
        public ChoiceCorrectTrainingModel()
        {
        }

        private Word currentWord = new Word { Spelling = "Hello" };

        public Word Word
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

        public ObservableCollection<string> Answers;


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
