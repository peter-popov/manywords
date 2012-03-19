using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
//using Microsoft.Phone.Data.Linq.Mapping;

namespace ManyWords.WordStorage
{
    [Table(Name = "Vocabulary")]
    public class Vocabulary : NotifyPropertyMembers
    {
        public Vocabulary()
        {
            this.words = new EntitySet<Word>(new Action<Word>(this.attach_vocabulary),
                                             new Action<Word>(this.detach_vocabulary));
        }

        [Column(IsVersion = true)]
        private Binary version;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ID;

        [Column]
        public string Description;

        [Column]
        public bool IsDefault;

        [Column(CanBeNull = false)]
        public string FromLanguage;

        [Column(CanBeNull = false)]
        public string ToLanguage;

        private EntitySet<Word> words;
        [Association(Storage = "words", OtherKey = "vocabID", ThisKey = "ID")]
        public EntitySet<Word> Words
        {
            get { return this.words; }
            set { this.words.Assign(value); }
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
    }
}
