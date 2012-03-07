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
using System.Collections.Generic;

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
        private WordsSelector wordSelector = new WordsSelector(App.WordStorage);
        private List<Word> trainingSet;
        private int wordsCount = 0;
        private int wordIndex = 0;
        

        public MultichoiceTrainingModel()
        {
            answers = new ObservableCollection<AnswerItemModel>();
            trainingSet = new List<Word>();
            trainingSet.AddRange(wordSelector.SelectWordsForTraining(10));
            wordIndex = -1;
        }


        public bool Next()
        {
            wordIndex++;
            if (wordIndex < trainingSet.Count)
            {
                Word = new WordListItemModel(trainingSet[wordIndex], null);

                var translations = wordSelector.SelectTranslations(trainingSet[wordIndex], 3);
                answers.Clear();

                answers.Add(new AnswerItemModel { Text = trainingSet[wordIndex].Translations[0].Spelling });
                foreach (Translation t in translations)
                {
                    answers.Add(new AnswerItemModel { Text = t.Spelling });                    
                }

                return true;
            }
            return false;
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
