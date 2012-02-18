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