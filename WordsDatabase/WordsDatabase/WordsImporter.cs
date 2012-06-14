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
                var row = line.Split('$');

                int pos = languageFrom.Item2;
                string spelling = row[pos].Trim();

                if (spelling == "")
                {
                    continue;
                }

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
                    Console.WriteLine("Warning: {0} - word ignored, no translations found", spelling);
                    continue;
                }

                Word item = new Word { Spelling = spelling, Added = DateTime.Now, State = State.New};
               
                database.Words.InsertOnSubmit(item);

                v.Words.Add(item);

                foreach (var t in translations)
                {
                    if (t.Item2.Trim().Length == 0)
                        continue;
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
            //
            // Create vocabulary and vocabulary info
            var newVocabulary = new Vocabulary { Description = vocabulary, 
                                                 IsPreloaded = true, 
                                                 IsUsed = true, 
                                                 Language = languageFrom.Item1 };
            //
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
