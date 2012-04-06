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
using System.Collections.Generic;
using ManyWords.WordStorage;


namespace ManyWords.Training
{

    public struct ApplicabilityInterval
    {
        public uint MinLevel;
        public uint MaxLevel;
        public uint Increment;
    }

    public class ExerciseInfo
    {
        public ApplicabilityInterval Info;
        public Type Model;
        public object Presenter;
    }


    public class TrainingController
    {
        public static uint MaxLevel = 100;
        private Random random = new Random((int)DateTime.Now.Ticks);
        private List<ExerciseInfo> exercies;
        private List<Word> words = new List<Word>();
        private ExerciseInfo currentExercise = null;
        private Model.Exercise currentExerciceModel = null;
        private int wordIndex = -1;
        
        public TrainingController(List<ExerciseInfo> exercies)
        {
            this.exercies = exercies;
        }

        public int NewWordsSeenCount { get; private set;}
        public int CorrectAnswersCount { get; private set; }
        public int WordsCount { get; private set; }
        public ExerciseInfo CurrentExercise
        {
            get
            {
                return currentExercise;
            }
        }


        public void StartNewTraining()
        {
            WordsSelector ws = new WordsSelector(App.WordStorage);
            words.AddRange(ws.SelectWordsForTraining(10));
            
            this.NewWordsSeenCount = 0;
            this.CorrectAnswersCount = 0;
            WordsCount = words.Count;
            wordIndex = -1;
        }

        public Model.Exercise Next()
        {            
            wordIndex++;
            if (wordIndex >= words.Count)
            {
                return null;
            }
            //
            //Need to select which exercise to use for current word
            List<ExerciseInfo> possibleExercises = SelectExercies(words[wordIndex].Level);
            System.Diagnostics.Debug.Assert(possibleExercises.Count != 0, "At least one exercie should be applicable");
            //
            //If have more than one use random
            currentExercise = possibleExercises[0];
            if (possibleExercises.Count > 1)
            {
                currentExercise = possibleExercises[random.Next(possibleExercises.Count)];
            }
            //
            // Create model
            currentExerciceModel = Activator.CreateInstance(currentExercise.Model, new object[] { words[wordIndex] }) as Model.Exercise;            
            //
            // TODO: this is hacking
            if (words[wordIndex].State == State.New)
            {
                NewWordsSeenCount++;
            }
            
            return currentExerciceModel;
        }

        public void CheckResult()
        {
            if (currentExerciceModel == null || currentExercise == null || wordIndex == -1 || wordIndex >= words.Count)
            {
                return;
            }

            Word currentWord = words[wordIndex];
            if (currentExerciceModel.Result)
            {
                currentWord.Level += currentExercise.Info.Increment;
                currentWord.Level = Math.Min(MaxLevel, currentWord.Level);
                CorrectAnswersCount++;
            }


            if (currentWord.State == State.Learning && currentWord.Level == MaxLevel)
            {
                currentWord.State = State.Learned;
            }
        }

        private List<ExerciseInfo> SelectExercies(uint level)
        {
            List<ExerciseInfo> res = new List<ExerciseInfo>();
            foreach (ExerciseInfo ex in exercies)
            {
                if (ex.Info.MinLevel <= level && ex.Info.MaxLevel > level)
                {
                    res.Add(ex);
                }
            }
            return res;
        }
    }
}
