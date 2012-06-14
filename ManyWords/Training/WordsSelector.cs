using System;
using System.Collections.Generic;
using System.Linq;

namespace ManyWords.WordStorage
{
    public class WordsSelector
    {
        private Storage storage;
        private static Random rnd = new Random((int)DateTime.Now.Ticks);
        private string languageStudy;
        private string languageMother;
        private Vocabulary vocabulary;

        public WordsSelector(Storage s, Vocabulary vocabulary)
        {
            this.storage = s;
            this.vocabulary = vocabulary;
            languageStudy = App.LanguagesListModel.StudyLanguage.Code;
            languageMother = App.LanguagesListModel.MotherLanguage.Code;            
        }


        public static IEnumerable<T> takeRandom<T>(IEnumerable<T> from)
        {
            return takeRandom(from, from.Count());
        }

        public static IEnumerable<T> takeRandom<T>(IEnumerable<T> from, int count)
        {
            count = Math.Min(count, from.Count());
            //
            // TODO: Think how to do it properly
            var list = from.ToList();
            var result = new List<T>();
            for (int i = 0; i < count; ++i)
            {
                int pos = rnd.Next(list.Count);
                result.Add(list[pos]);
                list.RemoveAt(pos);
            }

            return result;
        }

        private IEnumerable<Word> getWords()
        {
            if (vocabulary != null)
                return vocabulary.Words;
            else
                return storage.Words;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Word> SelectWordsForTraining(int count)
        {
            //select words in study language with translation to mother language avaliable
            var possibleWords = from Word w in getWords()
                                where (w.State == State.New || w.State == State.Learning) &&
                                       w.Vocabulary.Language == languageStudy &&
                                       w.Translations.Any(x => x.Language == languageMother)
                                orderby w.Level descending          
                                select w;
            //
            // Take 3 time more than requested, and select "count" randomly
            return takeRandom(possibleWords.Take(count * 3), count);
        }

        public IEnumerable<Translation> SelectTranslations(Word w, Translation main, int count)
        {
            int offset = rnd.Next(1, int.MaxValue/2);
            int mask = rnd.Next(10000, int.MaxValue);

            var res = from Translation t in storage.wordsDB.Translations
                      where t.Language == languageMother && 
                            t.Spelling != main.Spelling &&
                            t.wordID != w.WordID && 
                            //(t.Word.PartOfSpeech == w.PartOfSpeech || t.Word.PartOfSpeech == null) &&
                            (((t.ID + offset) ^ mask) % 10 > 5)                                   
                      select t;

            return res.Take(count);
        }

        public IEnumerable<Word> SelectWordsForTranslation(Translation t, int count)
        {
            int offset = rnd.Next(1, int.MaxValue / 2);
            int mask = rnd.Next(10000, int.MaxValue);

            var res = from Word w in storage.wordsDB.Words
                      where w.Vocabulary.Language == languageStudy && w.WordID != t.wordID && (((w.WordID + offset) ^ mask) % 10 > 5)
                      select w;

            return res.Take(count);
        }
    }
}
