using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
//using Microsoft.Phone.Data.Linq.Mapping;

namespace ManyWords.WordStorage
{
    //[Index(Columns = "Spelling ASC")]
    [Table(Name = "Translations")]
    public class Translation : NotifyPropertyMembers
    {
        [Column(IsVersion = true)]
        private Binary version;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID;

        [Column]
        public string Spelling;

        [Column]
        public int wordID;

        [Column]
        public string Language;

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
}
