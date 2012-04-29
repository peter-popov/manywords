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
using ManyWords.WordStorage;

namespace ManyWords.Model
{   
    public class NewWordExercise: Exercise
    {
        #region Bindings
        public SkipWord OnSkip { get; private set; }
        public LearnWord OnLearn { get; private set; }
        public string Word { get; private set; }
        public string Translation { get; private set; }
        #endregion

        private ICommand playSound = null;
        
        public NewWordExercise(Word word)
        {
            OnSkip = new SkipWord(word);
            OnLearn = new LearnWord(word);

            Word = word.Spelling;
            Translation = word.Translation;
            playSound = new PlaySound(word, App.TextToSpeech);
        }

        public override ICommand PlaySound
        {
            get
            {
                return playSound;
            }
        }

        public override ExerciseResult Result
        {
            get
            {
                return OnLearn.Result;
            }
        }

        public override void Ready()
        {
            if (PlaySound != null)
            {
                PlaySound.Execute(null);
            }
        }


        public class SkipWord : ICommand
        {
            Word word;
            public SkipWord(Word w)
            {
                this.word = w;
            }

            public bool CanExecute(object param)
            {
                return true;
            }

            public void Execute(object param)
            {
                word.State = State.Known;
            }

            public event EventHandler CanExecuteChanged;
        }

        public class LearnWord : ICommand
        {
            Word word;
            public LearnWord(Word w)
            {
                this.word = w;
                this.Result = ExerciseResult.Ignore;
            }

            public ExerciseResult Result { get; private set; }

            public bool CanExecute(object param)
            {
                return true;
            }

            public void Execute(object param)
            {
                word.State = State.Learning;
                Result = ExerciseResult.Repeat;
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
