using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ManyWords.WordStorage
{

    public class LearnStatistics
    {
        public bool Learned;
        public int Translated;
        public int TranslatedCorrectly;
    };

    public class Word
    {
        public string Spelling;
        public List<string> Translations;
        public string SpeachId;
        public LearnStatistics Satistics;
    };


    public class Storage
    {
        public Storage()
        {
        }

        public ObservableCollection<string> Tags;

        public IEnumerable<Word> Find(string word, bool look_in_translation = false)
        {
            return new List<Word>();
        }

        public IEnumerable<Word> FindTag(string tag)
        {
            return new List<Word>();
        }

        public Stream GetSpeachAudioStream(Word w)
        {
            return null;
        }

        public void StoreWord(Word w, Stream speach)
        {
            return;
        }
    }
}
