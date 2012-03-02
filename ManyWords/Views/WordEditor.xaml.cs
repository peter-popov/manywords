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
using System.Windows.Navigation;
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

        private bool isSave = false;
        private int wordId = -1;

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

        private void loadWord(int Id)
        {
            WordStorage.Word w = App.WordStorage.Find(Id);
            if (w == null) return;

            txtWord.Text = w.Spelling;
            wordId = Id;

            txtTranslations.Text = "";
            foreach (WordStorage.Translation t in w.Translations)
                txtTranslations.Text += t.Spelling + "\n";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("mode"))
            {
                if (NavigationContext.QueryString["mode"].ToLower() == "edit")
                {
                    if (NavigationContext.QueryString.ContainsKey("id"))
                    {
                        loadWord(int.Parse(NavigationContext.QueryString["id"]));
                    }
                    isSave = true;
                    this.PageTitle.Text = "edit word";
                    btnDone.Content = "Update";
                }
                else if (NavigationContext.QueryString["mode"].ToLower() == "new")
                {
                    isSave = false;
                    this.PageTitle.Text = "new word";
                    btnDone.Content = "Add";
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "wrong mode for page");
                }
            }
        }

        string clearWord(string s)
        {
            return s.Trim(new char[] { ' ', '.', '\\', '/', '\n' });
        }

        void doTranslate(string text)
        {
            if ( text.Length > 0 )
                default_translator.StartTranslate(text, from, to);         
        }

        bool isEnabled()
        {
            return clearWord(txtWord.Text).Length > 0 && clearWord(txtTranslations.Text).Length > 0;
        }

        private void txtWord_TextChanged(object sender, RoutedEventArgs e)
        {
            btnDone.IsEnabled = isEnabled();
            doTranslate( clearWord(txtWord.Text) );
        }

        private void txtWord_KeyDown(object sender, KeyEventArgs e)
        {
            btnDone.IsEnabled = isEnabled();            
        }

        private void txtTranslations_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnDone.IsEnabled = isEnabled();
        }

        private void txtTranslations_KeyDown(object sender, KeyEventArgs e)
        {
            btnDone.IsEnabled = isEnabled();
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            doTranslate(clearWord(txtWord.Text));
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
            if (e.IsOk == false )
            {
                //Do something
                return;
            }
            SoundEffect se = SoundEffect.FromStream(e.Result);
            se.Play();            
        }


        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            var wordText = clearWord(txtWord.Text);           
            var translations = txtTranslations.Text.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries );
            var clear_translations = from string s in translations
                                     where clearWord(s).Length > 0
                                     select clearWord(s);


            if (isSave)
            {
                App.WordStorage.StoreWord(wordId, wordText, clear_translations, null);                            
            }
            else
            {
                App.WordStorage.StoreWord(wordText, clear_translations, null);
            }

            NavigationService.GoBack();
        }
    }
}