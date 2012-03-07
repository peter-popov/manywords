using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace ManyWords.WordStorage
{
    public class WordsSelector
    {
        private Storage storage;
        private Random rnd = new Random((int)DateTime.Now.Ticks);


        public WordsSelector(Storage s)
        {
            this.storage = s;
        }


        private bool selector(Word w)
        {
            int sample = rnd.Next(100);

            return sample < 80;
        }



        public IEnumerable<Word> SelectWordsForTraining(int count)
        {

            return ( from Word w in storage.wordsDB.Words
                      where ( w.State == State.New || w.State == State.Learning )
                      orderby w.Completion descending
                      orderby w.Added
                      select w ).Take(count);
        }

        public IEnumerable<Translation> SelectTranslations(Word w, int count)
        {
            string main = w.Translations[0].Spelling;

            int offset = rnd.Next(1, int.MaxValue/2);
            int mask = rnd.Next(10000, int.MaxValue);

            var res = from Translation t in storage.wordsDB.Translations
                      where t.Spelling != main && t.wordID != w.WordID && (((t.ID + offset) ^ mask) % 10 > 5)
                      select t;

            return res.Take(count);
        }
    }
}
