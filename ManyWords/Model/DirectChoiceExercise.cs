using System.Linq;
using ManyWords.WordStorage;
using System.Windows.Input;

namespace ManyWords.Model
{
    public class DirectChoiceExercise : ChoiceExercise
    {
        private ChoiceAnswer correctAnswer;
        private ICommand playSound = null;

        public DirectChoiceExercise(Word word)
        {
            Question = new ChoiceQuestion { Text = word.Spelling };

            var correctTranslation = selectCorectTranslation(word);
            var wordSelector = new WordsSelector(App.WordStorage, null);
            var translations = wordSelector.SelectTranslations(word, correctTranslation, 3)
                                           .Select(x => new ChoiceAnswer { Text = x.Spelling, IsCorrect = false })
                                           .ToList();

            correctAnswer = new ChoiceAnswer { Text = correctTranslation.Spelling, IsCorrect = true };
            translations.Add(correctAnswer);

            foreach (var t in WordsSelector.takeRandom(translations))
            {
                Answers.Add(t);
            }

            playSound = new PlaySound(word, App.TextToSpeech);
        }

        public override ICommand PlaySound
        {
            get
            {
                return playSound;
            }
        }

        public override void Ready()
        {
            if (PlaySound != null)
            {
                PlaySound.Execute(null);
            }
        }
        
        public override void SubmitAnswer(ChoiceAnswer answer)
        {
            this.Result = ( answer.Text == correctAnswer.Text ) ? ExerciseResult.OK : ExerciseResult.Wrong;
        }
    }
}
