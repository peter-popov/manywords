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
        private Tuple<string, int> languageFrom;
        private List<Tuple<string, int>> languagesTo;


        public WordsImporter(WordsDB db, string vocabulary, Tuple<string, int> from, List<Tuple<string, int>> to)
        {
            this.database = db;
            this.vocabulary = vocabulary;
            this.languageFrom = from;
            this.languagesTo = to;
        }

        
        public void Import(string filePath)
        {
            Vocabulary v = findOrCreate();

            StreamReader sr = new StreamReader(filePath);            
            string line;
            Console.WriteLine("Creating database objects...");
            while ((line = sr.ReadLine()) != null)
            {
                var row = line.Split(',');

                int pos = languageFrom.Item2;
                string spelling = row[pos];
                var translations = new List<Tuple<string, string>>();

                foreach (var lng in languagesTo)
                {
                    pos = lng.Item2; 
                    if ( pos >= row.Length )
                    {
                        translations.Clear();
                        break;
                    }
                    translations.Add(Tuple.Create(lng.Item1, row[pos]));
                }

                if (translations.Count == 0)
                {
                    Console.WriteLine("Warning: {0} - word ignored, not all translations found", spelling);
                    continue;
                }

                Word item = new Word { Spelling = spelling, Added = DateTime.Now, State = State.New};
               
                database.Words.InsertOnSubmit(item);

                v.Words.Add(item);

                foreach (var t in translations)
                {
                    Translation tr = new Translation { Spelling = t.Item2, Language = t.Item1 };
                    item.Translations.Add(tr);
                    database.Translations.InsertOnSubmit(tr);
                }                                
            }
            
            Console.WriteLine("Submiting changes...");
            database.SubmitChanges();
        }


        private Vocabulary findOrCreate()
        {
            if (vocabulary == null)
            {                
                vocabulary = "default("+languageFrom.Item1+")";
            }
            /*else if ( vocabulary != null )
            {
                var res = (from Vocabulary v in database.Vocabularies
                          where (v.Description == vocabulary)
                          select v).FirstOrDefault();
                if (res != null)
                {
                    return res;
                }

                if (languageFrom == null )
                {
                    throw new ArgumentException("Target or source languages are not specified");
                }
            }*/

            // Create vocabulary and vocabulary info
            var newVocabulary = new Vocabulary { Description = vocabulary, 
                                                 IsPreloaded = true, 
                                                 IsUsed = true, 
                                                 Language = languageFrom.Item1 };

            foreach( var lng in languagesTo)
            {
                var targetLanguage = new VocabularyTargetLanguage { Language = lng.Item1 };
                newVocabulary.TargetLanguages.Add(targetLanguage);
                database.TargetLanguages.InsertOnSubmit(targetLanguage);               
            }
            database.Vocabularies.InsertOnSubmit(newVocabulary);
            return newVocabulary;
        }

    }
}
