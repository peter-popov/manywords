using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using ManyWords.WordStorage;

namespace WordsDatabase
{

    public class WordsDB : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Words.sdf";

        // Pass the connection string to the base class.
        public WordsDB(string connectionString)
            : base(connectionString)
        { }

        // Specify a single table for the to-do items.
        public Table<Word> Words;
        public Table<Translation> Translations;
        public Table<Vocabulary> Vocabularies;
    }
}
