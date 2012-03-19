using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
//using Microsoft.Phone.Data.Linq.Mapping;

namespace ManyWords.WordStorage
{
    public enum TrainingType
    {
        Unknown = 0,
        Multichoice = 1,
        Spelling = 2,
        Audio = 3
    }

    public enum State
    {
        Unknown = 0,
        New = 1,
        Learning = 2,
        Lerned = 3
    }

    /// <summary>
    /// 
    /// </summary>
    //[Index(Columns = "Spelling ASC, Added ASC")]
    [Table(Name = "Words")]
    public class Word : NotifyPropertyMembers
    {

        public Word()
        {
            this.translations = new EntitySet<Translation>(new Action<Translation>(this.attach_translation),
                                                           new Action<Translation>(this.detach_translation));
        }

        [Column(IsVersion = true)]
        private Binary version;

        #region BasicInfo
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

        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public DateTime Added
        { get; set; }

        private EntitySet<Translation> translations;
        [Association(Storage = "translations", OtherKey = "wordID", ThisKey = "WordID")]
        public EntitySet<Translation> Translations
        {
            get { return this.translations; }
            set { this.translations.Assign(value); }
        }
        #endregion

        #region LearnStatistics

        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public TrainingType LastTrainingType
        { get; set; }


        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public bool LastTrainingResult
        { get; set; }

        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public State State
        { get; set; }

        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Completion
        { get; set; }

        #endregion

        #region Vocabulary reference

        [Column]
        public int vocabID;

        private EntityRef<Vocabulary> vocab;
        [Association(Storage = "vocab", ThisKey = "vocabID", OtherKey = "ID", IsForeignKey = true)]
        public Vocabulary Vocabulary
        {
            get { return this.vocab.Entity; }
            set
            {
                NotifyPropertyChanging("Word");
                this.vocab.Entity = value;
                if (value != null)
                {
                    vocabID = value.ID;
                }
                NotifyPropertyChanged("Word");
            }
        }
        #endregion

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
}
