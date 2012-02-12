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
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ManyWords.WordStorage
{
    
    [Table]
    public class WordItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property and database column.
        private int wordId;
        [Column(IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int WordId
        {
            get
            {
                return wordId;
            }
            set
            {
                if (wordId != value)
                {
                    NotifyPropertyChanging("WordId");
                    wordId = value;
                    NotifyPropertyChanged("WordId");
                }
            }
        }


        private string spelling;
        [Column(IsPrimaryKey = true, CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string Spelling
        {
            get
            {
                return spelling;
            }
            set
            {
                if (spelling != value)
                {
                    NotifyPropertyChanging("Spelling");
                    spelling = value;
                    NotifyPropertyChanged("Spelling");
                }
            }
        }


        private string translation;
        [Column]
        public string Translation
        {
            get
            {
                return translation;
            }
            set
            {
                if (translation != value)
                {
                    NotifyPropertyChanging("Translation");
                    translation = value;
                    NotifyPropertyChanged("Translation");
                }
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }


    public class WordsDB : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/Words.sdf";

        // Pass the connection string to the base class.
        public WordsDB(string connectionString)
            : base(connectionString)
        { }

        // Specify a single table for the to-do items.
        public Table<WordItem> Words;
    }
}
