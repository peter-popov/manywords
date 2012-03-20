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

    public struct ExerciseInfo
    {
        public uint MinLevel;
        public uint MaxLevel;
        public uint Increment;
    }

    public struct Exercise
    {
        public ExerciseInfo Info;
        public Type Model;
    }


    public class TrainingController
    {
        private Random random = new Random((int)DateTime.Now.Ticks);
        private List<Exercise> exercies;
        private List<Word> words = new List<Word>();
        private int wordIndex = -1;

        public TrainingController(List<Exercise> exercies)
        {
            this.exercies = exercies;
        }

        public void StartNewTraining()
        {
            WordsSelector ws = new WordsSelector(App.WordStorage);
            words.AddRange(ws.SelectWordsForTraining(10));
            wordIndex = -1;
        }

        public object Next()
        {
            wordIndex++;
            if (wordIndex >= words.Count)
            {
                return null;
            }
            //
            //Need to select which exercise to use for current word
            List<Exercise> possibleExercises = SelectExercies(words[wordIndex].Level);
            if (possibleExercises.Count == 0)
            {
                //Something terrible should be done!!!
                return null;
            }
            //
            //If have more than one use random
            Exercise currentExercise = possibleExercises[0];
            if (possibleExercises.Count > 1)
            {
                currentExercise = possibleExercises[random.Next(possibleExercises.Count)];
            }
            //
            // Create model
            return Activator.CreateInstance(currentExercise.Model, new object[] { words[wordIndex] });
        }



        private List<Exercise> SelectExercies(uint level)
        {
            List<Exercise> res = new List<Exercise>();
            foreach (Exercise ex in exercies)
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
