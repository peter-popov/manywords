using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ManyWords.Views
{
    public partial class WordsView : PhoneApplicationPage
    {
        public WordsView()
        {
            InitializeComponent();
            DataContext = new Model.WordsViewModel();           
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WordStorage.Storage storage = App.WordStorage;

            System.Diagnostics.Debug.WriteLine("DUMP WORD");
            foreach (WordStorage.Word w in storage.Words)
            {
                System.Diagnostics.Debug.WriteLine("Id = {0}, Spelling = {1}", w.WordID, w.Spelling);
                //w.Translations.Load();
                foreach (WordStorage.Translation t in w.Translations)
                {
                    System.Diagnostics.Debug.WriteLine("\tId = {0}, Spelling = {1}, Word = {2}", t.ID, t.Spelling, t.wordID);
                }
            }

            System.Diagnostics.Debug.WriteLine("DUMP TRANSLATIONS");
            foreach (WordStorage.Translation t in storage.Translations)
            {
                System.Diagnostics.Debug.WriteLine("Id = {0}, Spelling = {1}, Word = {2}", t.ID, t.Spelling, t.wordID);
            }
        }

        private void Pivot_DoubleTap(object sender, GestureEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (allList.SelectedIndex == -1)
            {
                return;
            }

            var s = allList.SelectedItem;

            WordStorage.Storage storage = App.WordStorage;

            using (Stream audio = storage.GetSpeachAudioStream((s as Model.WordListItemModel).Word))
            {
                if (audio!=null)
                {
                    SoundEffect se = SoundEffect.FromStream(audio);
                    se.Play();
                }
            }

        }
    }
}