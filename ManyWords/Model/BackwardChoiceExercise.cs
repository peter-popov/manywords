using System.Linq;
using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class BackwardChoiceExercise: ChoiceExercise
    {
        private ChoiceAnswer correctAnswer;
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
        }



        public override void SubmitAnswer(ChoiceAnswer answer)
        {
            this.Result = (answer.Text == correctAnswer.Text);
        }
    }
}
