﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ManyWords.WordStorage
{
    public class WordsSelector
    {
        private Storage storage;
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private string languageStudy;
        private string languageMother;

        public WordsSelector(Storage s)
        {
            this.storage = s;
            languageStudy = App.LanguagesListModel.StudyLanguage.Code;
            languageMother = App.LanguagesListModel.MotherLanguage.Code;
        }


        private IEnumerable<T> takeRandom<T>(IEnumerable<T> from, int count)
        {
            if (from.Count() <= count)
            {
                return from;
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Word> SelectWordsForTraining(int count)
        {
            //select words in study language with translation to mother language avaliable
            var possibleWords = from Word w in storage.wordsDB.Words
                                where (w.State == State.New || w.State == State.Learning) &&
                                       w.Vocabulary.Language == languageStudy &&
                                       w.Translations.Any(x=>x.Language == languageMother)
                                orderby w.Level
                                orderby w.Added                        
                                select w;
            //
            // Take 5 time more than requested
            var selectSet = possibleWords.Take(count * 5);
            //
            // Randomize on result
            return takeRandom(selectSet, count);
        }

        public IEnumerable<Translation> SelectTranslations(Word w, int count)
        {
            string main = w.Translations[0].Spelling;

            int offset = rnd.Next(1, int.MaxValue/2);
            int mask = rnd.Next(10000, int.MaxValue);

            var res = from Translation t in storage.wordsDB.Translations
                      where t.Language == languageMother && t.Spelling != main && t.ID != w.WordID && (((t.ID + offset) ^ mask) % 10 > 5)
                      select t;

            return res.Take(count);
        }

        public IEnumerable<Word> SelectWordsForTranslation(Translation t, int count)
        {
            int offset = rnd.Next(1, int.MaxValue / 2);
            int mask = rnd.Next(10000, int.MaxValue);

            var res = from Word w in storage.wordsDB.Words
                      where w.Vocabulary.Language == languageStudy && w.WordID != t.ID && (((w.WordID + offset) ^ mask) % 10 > 5)
                      select w;

            return res.Take(count);
        }
    }
}
