using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class DirectChoiceExercise : ChoiceExercise
    {
        public DirectChoiceExercise(Word word)
        {
            Question = new ChoiceQuestion { Text = word.Spelling };

            var wordSelector = new WordsSelector(App.WordStorage);
            var translations = wordSelector.SelectTranslations(word, 3);

            Answers.Add(new ChoiceAnswer { Text = word.Translations[0].Spelling, IsCorrect = true });
            foreach (Translation t in translations)
            {
                Answers.Add(new ChoiceAnswer { Text = t.Spelling, IsCorrect = false });
            }                
        }
    }
}
