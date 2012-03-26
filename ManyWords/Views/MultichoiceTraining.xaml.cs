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
using Microsoft.Phone.Controls;
using ManyWords.Training;
using ManyWords.Model;

namespace ManyWords.Views
{
    public partial class MultichoiceTraining : PhoneApplicationPage
    {
        TrainingController trainingController;
        public MultichoiceTraining()
        {
            InitializeComponent();
            choiceControl.AnswerSelected += AnswerSelected;


            List<ExerciseInfo> exercises = new List<ExerciseInfo>();
            exercises.Add(new ExerciseInfo{Info = new ApplicabilityInterval{MinLevel = 0, MaxLevel = 100, Increment = 25}, Model = typeof(DirectChoiceExercise)});
            exercises.Add(new ExerciseInfo { Info = new ApplicabilityInterval { MinLevel = 25, MaxLevel = 100, Increment = 25 }, Model = typeof(BackwardChoiceExercise)});

            trainingController = new TrainingController(exercises);
        }



        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
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


        object oldDataContext = null;

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            trainingController.StartNewTraining();

            var model = trainingController.Next();
            if (model != null)
            {
                oldDataContext = DataContext;
                choiceControl.DataContext = model;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            GoToNext();
            btnNext.IsEnabled = false;
        }

        private void AnswerSelected(object sender, AnswerChoiceControl.AnswerSelectedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Selected answer: {0}", args.Index);
            trainingController.CheckResult();
            GoToNext();
        }

        private void GoToNext()
        {
            var model = trainingController.Next();
            if (model != null)
            {
                choiceControl.DataContext = model;
            }
            else
            {
                choiceControl.DataContext = oldDataContext;
            }
        }

    }
}