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

        Language from = new Language { Code = "de", Name = "German" };
        Language to = new Language { Code = "ru", Name = "Russian" };
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data           
            DataContext = App.TrainingsViewModel;

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            FrameworkDispatcher.Update();

            default_translator.TranslateComplete += TranslateCompleted;
            default_translator.SpeachReady += translator_SpeakCompleted;
        }        


        private void Button_Click(object sender, RoutedEventArgs e)
        {


            default_translator.StartTranslate(fromTxt.Text, from, to); 
        }

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            default_translator.StartSpeach(fromTxt.Text, from);
        }
        


        void TranslateCompleted(object sender, TranslatedEventArgs<List<string>> e)
        {
            toTxt.Text = "";
            foreach( string s in e.Result)
                toTxt.Text += s + "\n";
        }


        bool is_save_mode = false;

        void translator_SpeakCompleted(object sender, TranslatedEventArgs<Stream> e)
        {
            if (!is_save_mode)
            {
                //SoundEffect se = SoundEffect.FromStream(e.Result);
                //se.Play();
            }
            else
            {
                using (WordStorage.Storage storage = new WordStorage.Storage())
                {
                    WordStorage.Word new_word = new WordStorage.Word { Spelling = fromTxt.Text };
                    

                    storage.StoreWord(fromTxt.Text, toTxt.Text.Split(new char[]{'\n'}), e.Result);
                }
            }

            is_save_mode = false;
        }

        private void btnWords_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/Views/WordsView.xaml", UriKind.Relative);
            System.Diagnostics.Debug.WriteLine(uri.ToString());
            NavigationService.Navigate(uri);
        }

       
        private void btnLearn_Click(object sender, RoutedEventArgs e)
        {
            is_save_mode = true;
            default_translator.StartSpeach(toTxt.Text, to);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/Views/WordEditor.xaml", UriKind.Relative);
            System.Diagnostics.Debug.WriteLine(uri.ToString());
            NavigationService.Navigate(uri);
        }
    }
}