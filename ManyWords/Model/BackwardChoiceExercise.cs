using ManyWords.WordStorage;

namespace ManyWords.Model
{
    public class BackwardChoiceExercise: ChoiceExercise
    {
        public BackwardChoiceExercise(Word word)
        {
            var translation = word.Translations[0];
            Question = new ChoiceQuestion { Text = translation.Spelling };

            var wordSelector = new WordsSelector(App.WordStorage);
            var words = wordSelector.SelectWordsForTranslation(translation, 3);

            Answers.Add(new ChoiceAnswer { Text = word.Spelling, IsCorrect = true });
            foreach (Word w in words)
            {
                Answers.Add(new ChoiceAnswer { Text = w.Spelling, IsCorrect = false });
            }              
        }
    }
}
