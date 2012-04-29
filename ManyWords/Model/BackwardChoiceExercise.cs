using System.Linq;
using ManyWords.WordStorage;
using System.Windows.Input;

namespace ManyWords.Model
{
    public class BackwardChoiceExercise: ChoiceExercise
    {
        private ChoiceAnswer correctAnswer;
        private ICommand playSound = null;

        public BackwardChoiceExercise(Word word)
        {
            var translation = selectCorectTranslation(word);
            Question = new ChoiceQuestion { Text = translation.Spelling };

            var wordSelector = new WordsSelector(App.WordStorage);
            var words = wordSelector.SelectWordsForTranslation(translation, 3)
                                    .Select(x=>new ChoiceAnswer { Text = x.Spelling, IsCorrect = false })
                                    .ToList();

            correctAnswer = new ChoiceAnswer { Text = word.Spelling, IsCorrect = true };
            words.Add(correctAnswer);
            foreach (var answer in WordsSelector.takeRandom(words))
            {
                Answers.Add(answer);
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

        public override void SubmitAnswer(ChoiceAnswer answer)
        {
            if (PlaySound != null)
            {
                PlaySound.Execute(null);
            }

            this.Result = (answer.Text == correctAnswer.Text) ? ExerciseResult.OK : ExerciseResult.Wrong;
        }
    }
}
