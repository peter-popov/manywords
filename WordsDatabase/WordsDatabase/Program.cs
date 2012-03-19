using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace WordsDatabase
{
    using Command = Tuple<string, Action<string[]>>;

    class Program
    {

        
        static Dictionary<string,  Command> commands;
        static Program()
        {

            commands = new Dictionary<string, Tuple<string, Action<string[]>>>();


            commands.Add("create", new Command("Creates new empty database", cmd_create));
            commands.Add("import", new Command("Imports words from CSV file(s) into database", cmd_import));
            commands.Add("show", new Command("Print database content", cmd_show));

        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            var commandName = args[0];

            if (!commands.ContainsKey(commandName))
            {
                return;
            }

            commands[commandName].Item2(args.Skip(1).ToArray());
        }



        static void cmd_create(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                //print usage
                return;
            }

            createDatabase(args[0]);            
        }


        static void cmd_import(string[] args)
        {
            if (args == null )
            {
                //print usage
                return;
            }

            string vocabulary = null;
            string fromLanguge = null;
            string toLanguage = null;
            List<string> files = new List<string>();

            var p = new OptionSet() {
                            { "vocab=", 
                              "vocabulary name to add new words, if not provided new/default vocabulary will be used",
                              (string v) => vocabulary = v },
                            { "f|file=", 
                              "CSV file to be loaded. Can be specified multiple times.",
                              (string v) => files.Add(v) },
                            { "s|source-language=", 
                              "source language code",
                               (string v) => fromLanguge = v },
                            { "t|target-language=",  
                              "target language code", 
                               (string v) => toLanguage = v },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("WordsDatabase: ");
                Console.WriteLine(e.Message);
                return;
            }

            if (extra.Count == 0)
            {
                Console.WriteLine("Database path is not specified");
                return;
            }

            if (vocabulary != null || (fromLanguge != null && toLanguage != null))
            {
                WordsDB wordsDB = createDatabase(extra[0]);

                WordsImporter importer = new WordsImporter(wordsDB, vocabulary, fromLanguge, toLanguage);

                foreach (string f in files)
                {
                    importer.Import(f);
                }
            }
            else
            {
                Console.WriteLine("Either vocabulary name or source and target language must be specified");
            }
        }


        static void cmd_show(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                //print usage
                return;
            }

            WordsDB db = createDatabase(args[0]);

            StatisticsCollector stat = new StatisticsCollector(db);
            stat.Dump();
        }


        static WordsDB createDatabase(string path)
        {
            // Create the database if it does not exist.
            WordsDB wordsDB = new WordsDB(path);
            if (wordsDB.DatabaseExists() == false)
            {
                //Create the database
                wordsDB.CreateDatabase();
            }
            return wordsDB;
        }
    }
}
