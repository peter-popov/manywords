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

        public NewWordExercise(Word word)
        {
            OnSkip = new SkipWord(word);
            OnLearn = new LearnWord(word);

            Word = word.Spelling;
            Translation = word.Translation;
        }

        public override bool Result
        {
            get
            {
                return OnLearn.Result;
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
                word.State = State.Learned;
                System.Diagnostics.Debug.WriteLine("SkipWord::Execute");
            }

            public event EventHandler CanExecuteChanged;
        }

        public class LearnWord : ICommand
        {
            Word word;
            public LearnWord(Word w)
            {
                this.word = w;
                this.Result = false;
            }

            public bool Result { get; private set; }

            public bool CanExecute(object param)
            {
                return true;
            }

            public void Execute(object param)
            {
                word.State = State.Learning;
                Result =true;
                System.Diagnostics.Debug.WriteLine("LearnWord::Execute");
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
