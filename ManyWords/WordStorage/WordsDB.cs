using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;

namespace ManyWords.WordStorage
{
    
    public class WordsDB : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/Words.sdf";

        //public static string DBConnectionString = "Data Source = 'appdata:/Words.sdf'; File Mode = read only;";

        // Pass the connection string to the base class.
        public WordsDB(string connectionString)
            : base(connectionString)
        { }

        // Specify tables
        public Table<Word> Words;
        public Table<Translation> Translations;
        public Table<Vocabulary> Vocabularies;
        public Table<VocabularyTargetLanguage> TargetLanguages;
    }
}
