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
    public class SpellingExercise: Exercise
    {
        #region Bindings
        public string Word { get; private set; }
        public string Translation { get; private set; }        
        #endregion

       
        public SpellingExercise(Word word)
        {
            Word = word.Spelling;
            Translation = word.getTranslation(App.LanguagesListModel.MotherLanguage.Code);
        }

        public void SubmitAnswer(string answer)
        {
            Result = ( String.Compare(answer.Trim(), Word, StringComparison.InvariantCultureIgnoreCase) == 0 ) ?
                ExerciseResult.OK : ExerciseResult.Wrong;
        }

    }
}
