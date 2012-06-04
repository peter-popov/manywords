using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace ManyWords.WordStorage
{
    public enum PartsOfSpeech
    {    
        Noun,
        Verb,
        Adjective,
        Adverb,
        Pronoun,
        Preposition,
        Interjection,
        Conjunction
    }
            
    public enum Genders
    {
        Femine,
        Musculin,
        Neutral,
    }

    public enum State
    {
        New = 0,
        Learning = 1,
        Learned = 2,
        Known = 3
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

        [Column(CanBeNull = true, AutoSync = AutoSync.OnInsert)]
        public PartsOfSpeech PartOfSpeech
        { get; set; }

        [Column(CanBeNull = true, AutoSync = AutoSync.OnInsert)]
        public Genders Gender
        { get; set; }

        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public DateTime Added
        { get; set; }

        [Column(CanBeNull = true, AutoSync = AutoSync.OnInsert)]
        public string SpecialForms
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

        private uint level;
        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public uint Level
        {
            get
            {
                return level;
            }
            set
            {
                if (level != value)
                {
                    NotifyPropertyChanging("Level");
                    level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }

        private State state;
        [Column(CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public State State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    NotifyPropertyChanging("State");
                    state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

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
                NotifyPropertyChanging("Vocabulary");
                this.vocab.Entity = value;
                if (value != null)
                {
                    vocabID = value.ID;
                }
                NotifyPropertyChanged("Vocabulary");
            }
        }
        #endregion

        #region Non DB properties
        public string AudioFile
        {
            get
            {
                return WordID.ToString().Trim() + ".wav";
            }
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
                        if (s.Length > 0) s += "; ";
                        s += t.Spelling;
                    }
                    return s;
                }
                else
                    return "";
            }
        }
        #endregion


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
