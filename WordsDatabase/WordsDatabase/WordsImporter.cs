using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyWords.WordStorage;
using System.IO;

namespace WordsDatabase
{
    class WordsImporter
    {
        private WordsDB database;
        private string vocabulary;
        private string languageFrom;
        private string languageTo;


        public WordsImporter(WordsDB db, string vocabulary, string from, string to)
        {
            this.database = db;
            this.vocabulary = vocabulary;
            this.languageFrom = from;
            this.languageTo = to;
        }

        public void Import(string filePath)
        {
            Vocabulary v = findOrCreate();

            StreamReader sr = new StreamReader(filePath, Encoding.Default);            
            string line;
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (i++ % 25 == 0)
                {
                    Console.WriteLine("On {0}...", i);
                }

                var row = line.Split(',');

                string spelling = row[0];
                string translation = row[1];

                Word item = new Word { Spelling = spelling, Added = DateTime.Now, State = State.New};
               
                database.Words.InsertOnSubmit(item);

                v.Words.Add(item);

                Translation tr = new Translation { Spelling = translation};
                item.Translations.Add(tr);
                database.Translations.InsertOnSubmit(tr);                
            }

            database.SubmitChanges();
        }


        private Vocabulary findOrCreate()
        {
            if (vocabulary == null && languageFrom != null && languageTo != null)
            {                
                var res = ( from Vocabulary v in database.Vocabularies
                          where ( v.FromLanguage == languageFrom && v.ToLanguage == languageTo && v.IsDefault)
                          select v ).FirstOrDefault();
                if (res != null)
                {
                    return res;
                }
                vocabulary = "default("+languageFrom+"->"+languageTo+")";
            }
            else if ( vocabulary != null )
            {
                var res = (from Vocabulary v in database.Vocabularies
                          where (v.Description == vocabulary)
                          select v).FirstOrDefault();
                if (res != null)
                {
                    return res;
                }

                if (languageFrom == null || languageTo == null)
                {
                    throw new ArgumentException("Targte or source languages are not specified");
                }
            }

            // If not found need to create
            var newVocabulary = new Vocabulary { Description = vocabulary, IsDefault = true, FromLanguage = languageFrom, ToLanguage = languageTo };

            database.Vocabularies.InsertOnSubmit(newVocabulary);
            return newVocabulary;
        }

    }
}
