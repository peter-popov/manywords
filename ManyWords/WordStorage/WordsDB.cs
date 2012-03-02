﻿using System;
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
using Microsoft.Phone.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ManyWords.WordStorage
{
    public class NotifyPropertyMembers: INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        protected void NotifyPropertyChanged(string propertyName)
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
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [Index(Columns = "Spelling ASC")]
    [Table(Name = "Words")]
    public class Word : NotifyPropertyMembers
    {       
        private EntitySet<Translation> translations;
        public Word()
		{
            this.translations = new EntitySet<Translation>(new Action<Translation>(this.attach_translation), 
                                                           new Action<Translation>(this.detach_translation));
		}

        private int wordId;
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int WordID
        {
            get
            {
                return wordId;
            }
            set
            {
                if (wordId != value)
                {
                    NotifyPropertyChanging("WordID");
                    wordId = value;
                    NotifyPropertyChanged("WordID");
                }
            }
        }


        private string spelling;
        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]        
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


        [Association(Storage = "translations", OtherKey = "wordID", ThisKey = "WordID")]
        public EntitySet<Translation> Translations
        {
            get { return this.translations; }
            set { this.translations.Assign(value); }
        }

        public string Translation
        {
            get
            {
                if (translations != null)
                {
                    string s = "";
                    foreach (Translation t in translations)
                    {
                        s += t.Spelling + "; ";
                    }
                    return s;
                }
                else
                    return "";
            }            
        }


        private void attach_translation(Translation entity)
        {
            NotifyPropertyChanging("Translation");
            entity.Word = this;
        }

        private void detach_translation(Translation entity)
        {
            NotifyPropertyChanging("Translation");
            entity.Word = null;
        }
    }


    [Table(Name = "Translations")]
    public class Translation : NotifyPropertyMembers
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID;

        [Column]
        public string Spelling;

        [Column]
        public int wordID;

        
        private EntityRef<Word> word;
        [Association(Storage = "word", ThisKey = "wordID", OtherKey = "WordID", IsForeignKey = true)]
        public Word Word
        {
            get { return this.word.Entity; }
            set 
            {
                NotifyPropertyChanging("Word");
                this.word.Entity = value;
                if (value != null)
                {
                    wordID = value.WordID;
                }
                NotifyPropertyChanged("Word");
            }
        }
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
        public Table<Word> Words;
        public Table<Translation> Translations;
    }
}
