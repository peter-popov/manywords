using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class BackwardChoiceExercise: ChoiceExercise
    {
        private ChoiceAnswer correctAnswer;
        public BackwardChoiceExercise(Word word)
        {
            var translation = word.Translations[0];
            Question = new ChoiceQuestion { Text = translation.Spelling };

            var wordSelector = new WordsSelector(App.WordStorage);
            var words = wordSelector.SelectWordsForTranslation(translation, 3);

            correctAnswer = new ChoiceAnswer { Text = word.Spelling, IsCorrect = true };
            Answers.Add(correctAnswer);
            foreach (Word w in words)
            {
                Answers.Add(new ChoiceAnswer { Text = w.Spelling, IsCorrect = false });
            }              
        }


        public override void SubmitAnswer(ChoiceAnswer answer)
        {
            this.Result = (answer.Text == correctAnswer.Text);
        }
    }
}
