using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using ManyWords.Training;
using ManyWords.Model;

namespace ManyWords.Views
{
    public partial class TrainingView : PhoneApplicationPage
    {
        TrainingController trainingController;
        public TrainingView()
        {
            InitializeComponent();
            choiceControl.AnswerSelected += AnswerSelected;
            presentControl.AnswerSelected += AnswerSelected;
            spellingControl.AnswerSelected += AnswerSelected;

            List<ExerciseInfo> exercises = new List<ExerciseInfo>();
            exercises.Add(new ExerciseInfo
            {
                Info = new ApplicabilityInterval { MinLevel = 0, MaxLevel = 1, Increment = 1 },
                Model = typeof(NewWordExercise),
                Presenter = presentControl
            });

            exercises.Add(new ExerciseInfo
            {
                Info = new ApplicabilityInterval { MinLevel = 1, MaxLevel = 60, Increment = 20 },
                Model = typeof(DirectChoiceExercise),
                Presenter = choiceControl
            });

            exercises.Add(new ExerciseInfo
            {
                Info = new ApplicabilityInterval { MinLevel = 60, MaxLevel = 90, Increment = 10 },
                Model = typeof(BackwardChoiceExercise),
                Presenter = choiceControl
            });

            exercises.Add(new ExerciseInfo
            {
                Info = new ApplicabilityInterval { MinLevel = 70, MaxLevel = 100, Increment = 10 },
                Model = typeof(SpellingExercise),
                Presenter = spellingControl
            });

            trainingController = new TrainingController(exercises);
        }

        WordStorage.Vocabulary vocabulary = null;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //vocabulary_id
            if (NavigationContext.QueryString.ContainsKey("vocabulary_id"))
            {
                int id;
                if (int.TryParse(NavigationContext.QueryString["vocabulary_id"], out id))
                {
                    vocabulary = App.WordStorage.FindVocabulary(id);
                }
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (trainingController != null)
            {
                trainingController.CheckResult();
                App.WordStorage.wordsDB.SubmitChanges();
                App.VocabularyListModel.Update();
            }
            base.OnNavigatedFrom(e);
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            performanceProgressBar.IsIndeterminate = true;
            progressPanel.Visibility = System.Windows.Visibility.Visible;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += LoadWords;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WordsLoadingCompleted);
            bw.RunWorkerAsync();
        }

        private void LoadWords(object sender, DoWorkEventArgs e)
        {
            trainingController.StartNewTraining(vocabulary);        
        }

        void WordsLoadingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            performanceProgressBar.IsIndeterminate = false;
            progressPanel.Visibility = System.Windows.Visibility.Collapsed;
            GoToNext();
        }

        private void AnswerSelected(object sender, EventArgs args)
        {
            trainingController.CheckResult();
            GoToNext();
        }

        private ExerciseControl lastView;
        private void GoToNext()
        {
            if (lastView != null)
            {
                lastView.Visibility = Visibility.Collapsed;
            }

            var model = trainingController.Next();
            if (model != null)
            {
                lastView = trainingController.CurrentExercise.Presenter as ExerciseControl;
                lastView.DataContext = model;
                model.Ready();
                lastView.Reset();
                lastView.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                App.WordStorage.wordsDB.SubmitChanges();
                ctlStatistic.Visibility = System.Windows.Visibility.Visible;
                txtNewWords.Text = trainingController.NewWordsSeenCount.ToString();
                txtAnswers.Text = string.Format("{0} of {1}", trainingController.CorrectAnswersCount, trainingController.WordsCount);
            }
        }

        private void ContentPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }


    }
}