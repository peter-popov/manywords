using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyWords.WordStorage;
using System.IO;

namespace WordsDatabase
{
    class StatisticsCollector
    {
        WordsDB database;
        public StatisticsCollector(WordsDB db)
        {
            this.database = db;
        }


        public void Dump()
        {            
            foreach (Word w in database.Words)
            {
                Console.WriteLine("{0},{1},{2},{3}",
                    w.Spelling, w.Translation, w.Vocabulary.Description, w.Added);
            }
        }
    }
}
