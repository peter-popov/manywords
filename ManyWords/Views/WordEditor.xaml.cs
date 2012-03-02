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

        #region State
        ITranslator default_translator = TranslatorFactory.CreateInstance();
        WordStorage.Storage storage = App.WordStorage;

        private bool isSave = false;
        private int wordId = -1;

        byte[] audioCache = null;
        string lastText = null;

        Language from = new Language { Code = "de", Name = "German" };
        Language to = new Language { Code = "ru", Name = "Russian" };
        #endregion

        #region Initialization
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
            WordStorage.Word w = storage.Find(Id);
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
        #endregion

        #region Translation
        void doTranslate(string text)
        {
            if (text.Length > 0)
                default_translator.StartTranslate(text, from, to);
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
        #endregion


        void translator_SpeakCompleted(object sender, TranslatedEventArgs<Stream> e)
        {            
            if (e.IsOk == false )
            {
                //MessageBox.Show("Audio is not available at the moment");
                return;
            }
            cacheSpeech(e.Result);
            playBuffer(new MemoryStream(audioCache));
        }

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            if (lastText != null && lastText == clearWord(txtWord.Text) && audioCache != null)
            {
                playBuffer(new MemoryStream(audioCache));                
            }
            else
            {
                default_translator.StartSpeach(clearWord(txtWord.Text), from);
                lastText = clearWord(txtWord.Text);
            }
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            var wordText = clearWord(txtWord.Text);           
            var translations = txtTranslations.Text.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries );
            var clear_translations = from string s in translations
                                     where clearWord(s).Length > 0
                                     select clearWord(s);

            Stream audioToStore = null;
            if (lastText == wordText && audioCache != null)
            {
                audioToStore = new MemoryStream(audioCache);
            }

            if (isSave)
            {
                storage.StoreWord(wordId, wordText, clear_translations, audioToStore);                            
            }
            else
            {
                storage.StoreWord(wordText, clear_translations, audioToStore);
            }

            NavigationService.GoBack();
        }

        #region helreps
        string clearWord(string s)
        {
            return s.Trim(new char[] { ' ', '.', '\\', '/', '\n' });
        }

        bool isEnabled()
        {
            return clearWord(txtWord.Text).Length > 0 && clearWord(txtTranslations.Text).Length > 0;
        }

        void playBuffer(Stream s)
        {
            SoundEffect se = SoundEffect.FromStream(s);
            se.Play();
        }

        void cacheSpeech(Stream s)
        {
            audioCache = new byte[s.Length];
            MemoryStream ms = new MemoryStream(audioCache);
            s.CopyTo(ms);
        }
        #endregion
    }
}