using System.Linq;
using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class DirectChoiceExercise : ChoiceExercise
    {
        private ChoiceAnswer correctAnswer;
        public DirectChoiceExercise(Word word)
        {
            Question = new ChoiceQuestion { Text = word.Spelling };

            var correctTranslation = selectCorectTranslation(word);
            var wordSelector = new WordsSelector(App.WordStorage);
            var translations = wordSelector.SelectTranslations(word, correctTranslation, 3)
                                           .Select(x => new ChoiceAnswer { Text = x.Spelling, IsCorrect = false })
                                           .ToList();

            correctAnswer = new ChoiceAnswer { Text = correctTranslation.Spelling, IsCorrect = true };
            translations.Add(correctAnswer);

            foreach (var t in WordsSelector.takeRandom(translations))
            {
                Answers.Add(t);
            }
        }



        public override void SubmitAnswer(ChoiceAnswer answer)
        {
            this.Result = ( answer.Text == correctAnswer.Text );
        }
    }
}
