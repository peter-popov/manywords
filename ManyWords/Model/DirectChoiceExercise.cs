using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class DirectChoiceExercise : ChoiceExercise
    {
        private ChoiceAnswer correctAnswer;
        public DirectChoiceExercise(Word word)
        {
            Question = new ChoiceQuestion { Text = word.Spelling };

            var wordSelector = new WordsSelector(App.WordStorage);
            var translations = wordSelector.SelectTranslations(word, 3);

            correctAnswer = new ChoiceAnswer { Text = word.Translations[0].Spelling, IsCorrect = true };
            Answers.Add(correctAnswer);
            foreach (Translation t in translations)
            {
                Answers.Add(new ChoiceAnswer { Text = t.Spelling, IsCorrect = false });
            }                
        }


        public override void SubmitAnswer(ChoiceAnswer answer)
        {
            this.Result = ( answer.Text == correctAnswer.Text );
        }
    }
}
