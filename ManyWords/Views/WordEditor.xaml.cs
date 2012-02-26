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
using ManyWords.Translator;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ManyWords.Views
{
    public partial class WordEditor : PhoneApplicationPage
    {
        ITranslator default_translator = TranslatorFactory.CreateInstance();

        Language from = new Language { Code = "de", Name = "German" };
        Language to = new Language { Code = "ru", Name = "Russian" };

        public WordEditor()
        {
            InitializeComponent();
        }


        private void WordEditor_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update();

            default_translator.TranslateComplete += translator_TranslateCompleted;
            default_translator.SpeachReady += translator_SpeakCompleted;

            txtWord.TextInput += txtWord_TextChanged;
        }

        private void txtWord_TextChanged(object sender, RoutedEventArgs e)
        {
            default_translator.StartTranslate(txtWord.Text, from, to);     
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            default_translator.StartTranslate(txtWord.Text, from, to); 
        }

        void translator_TranslateCompleted(object sender, TranslatedEventArgs<List<string>> e)
        {
            if (e.IsOk == false)
            {
                //Do something
                return;
            }
            txtTranslations.Text = "";
            e.Result.ForEach( s => txtTranslations.Text += s + "\n" );            
        }
        

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            default_translator.StartSpeach(txtWord.Text, from);
        }

        void translator_SpeakCompleted(object sender, TranslatedEventArgs<Stream> e)
        {
            if (e.IsOk == false)
            {
                //Do something
                return;
            }
            SoundEffect se = SoundEffect.FromStream(e.Result);
            se.Play();            
        }


        private void btnDone_Click(object sender, RoutedEventArgs e)
        {

        }



    }
}