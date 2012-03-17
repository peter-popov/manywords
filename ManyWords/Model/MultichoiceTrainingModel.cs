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

    public class MultichoiceTrainingModel: INotifyPropertyChanged
    {
        private WordsSelector wordSelector = new WordsSelector(App.WordStorage);
        private List<Word> trainingSet;
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
                Translation translation = trainingSet[wordIndex].Translations[0];
                answers.Clear();

                if (wordIndex % 2 == 0)
                {
                    Word = new AnswerItemModel{Text = trainingSet[wordIndex].Spelling};
                    
                    var translations = wordSelector.SelectTranslations(trainingSet[wordIndex], 3);
                    
                    answers.Add(new AnswerItemModel { Text = translation.Spelling, IsCorrect = true });
                    foreach (Translation t in translations)
                    {
                        answers.Add(new AnswerItemModel { Text = t.Spelling, IsCorrect = false });
                    }
                }
                else
                {
                    Word = new AnswerItemModel { Text = translation.Spelling };

                    var words = wordSelector.SelectWordsForTranslation(translation, 3);

                    answers.Add(new AnswerItemModel { Text = trainingSet[wordIndex].Spelling, IsCorrect = true });
                    foreach (Word w in words)
                    {
                        answers.Add(new AnswerItemModel { Text = w.Spelling, IsCorrect = false });
                    }
                }

                return true;
            }
            return false;
        }


        private AnswerItemModel currentWord;
        public AnswerItemModel Word
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
