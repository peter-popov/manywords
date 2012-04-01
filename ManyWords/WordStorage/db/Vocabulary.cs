using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
//using Microsoft.Phone.Data.Linq.Mapping;

namespace ManyWords.WordStorage
{
    [Table(Name = "VocabularyTargetLanguage")]
    public class VocabularyTargetLanguage : NotifyPropertyMembers
    {
        [Column(IsVersion = true)]
        private Binary version;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID;

        [Column]        
        public string Language;

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
    }


    [Table(Name = "Vocabulary")]
    public class Vocabulary : NotifyPropertyMembers
    {
        public Vocabulary()
        {
            this.words = new EntitySet<Word>(new Action<Word>(this.attach_vocabulary),
                                             new Action<Word>(this.detach_vocabulary));

            this.targetLanguages = new EntitySet<VocabularyTargetLanguage>(
                                                new Action<VocabularyTargetLanguage>(this.attach_target_language),
                                                new Action<VocabularyTargetLanguage>(this.detach_target_language));            
        }

        [Column(IsVersion = true)]
        private Binary version;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID;

        [Column]
        public string Description;

        [Column(CanBeNull = false)]
        public bool IsPreloaded;

        [Column(CanBeNull = false)]
        public bool IsUsed;

        [Column(CanBeNull = false)]
        public string Language;

        private EntitySet<Word> words;
        [Association(Storage = "words", OtherKey = "vocabID", ThisKey = "ID")]
        public EntitySet<Word> Words
        {
            get { return this.words; }
            set { this.words.Assign(value); }
        }

        private EntitySet<VocabularyTargetLanguage> targetLanguages;
        [Association(Storage = "targetLanguages", OtherKey = "vocabID", ThisKey = "ID")]
        public EntitySet<VocabularyTargetLanguage> TargetLanguages
        {
            get { return this.targetLanguages; }
            set { this.targetLanguages.Assign(value); }
        }

        private void attach_vocabulary(Word entity)
        {
            NotifyPropertyChanging("Words");
            entity.Vocabulary = this;
        }

        private void detach_vocabulary(Word entity)
        {
            NotifyPropertyChanging("Words");
            entity.Vocabulary = null;
        }

        private void attach_target_language(VocabularyTargetLanguage entity)
        {
            NotifyPropertyChanging("TargetLanguages");
            entity.Vocabulary = this;
        }

        private void detach_target_language(VocabularyTargetLanguage entity)
        {
            NotifyPropertyChanging("TargetLanguages");
            entity.Vocabulary = null;
        }
    }
}
