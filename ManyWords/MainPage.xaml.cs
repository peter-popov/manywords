using System;
using System.IO;
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using ManyWords.Translator;

namespace ManyWords
{
    public partial class MainPage : PhoneApplicationPage
    {

        ITranslator default_translator = TranslatorFactory.CreateInstance();

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update();

            default_translator.TranslateComplete += TranslateCompleted;
            default_translator.SpeachReady += translator_SpeakCompleted;

            fromLng.ItemsSource = default_translator.Languages;
            toLng.ItemsSource = default_translator.Languages;
        }        


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Language from = fromLng.SelectedItem as Language;
            Language to = toLng.SelectedItem as Language;

            default_translator.StartTranslate(fromTxt.Text, from, to); 
        }

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            Language to = toLng.SelectedItem as Language;

            default_translator.StartSpeach(toTxt.Text, to);
        }



        void TranslateCompleted(object sender, TranslatedEventArgs<string> e)
        {
            toTxt.Text = e.Result;
        }

        void translator_SpeakCompleted(object sender, TranslatedEventArgs<Stream> e)
        {
            SoundEffect se = SoundEffect.FromStream(e.Result);
            se.Play();
        }
    }
}