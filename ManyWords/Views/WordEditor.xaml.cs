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

namespace ManyWords.Views
{
    public partial class WordEditor : PhoneApplicationPage
    {

        #region State
        ITranslator default_translator = TranslatorFactory.CreateInstance();
        Model.TextToSpeech textToSpeech;
        WordStorage.Storage storage = App.WordStorage;

        private bool isSave = false;
        private int wordId = -1;

        Language languageFrom = null;
        Language languageTo = null;
        #endregion

        #region Initialization
        public WordEditor()
        {
            InitializeComponent();
            languageFrom = new Language
            {
                Name = App.LanguagesListModel.StudyLanguage.Name,
                Code = App.LanguagesListModel.StudyLanguage.Code
            };
            textToSpeech = new Model.TextToSpeech(languageFrom, default_translator);
        }

        private void WordEditor_Loaded(object sender, RoutedEventArgs e)
        {           
            default_translator.TranslateComplete += translator_TranslateCompleted;            
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

            var item = App.VocabularyListModel.All.FirstOrDefault(x => x.Vocabulary.ID == w.vocabID);
            if (item != null)
                vocabularyPicker.SelectedItem = item;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            languageFrom = new Language
            {
                Name = App.LanguagesListModel.StudyLanguage.Name,
                Code = App.LanguagesListModel.StudyLanguage.Code
            };

            languageTo = new Language
            {
                Name = App.LanguagesListModel.MotherLanguage.Name,
                Code = App.LanguagesListModel.MotherLanguage.Code
            };

            if (NavigationContext.QueryString.ContainsKey("mode"))
            {
                vocabularyPicker.ItemsSource = App.VocabularyListModel.All;
                if (NavigationContext.QueryString["mode"].ToLower() == "edit")
                {                    
                    if (NavigationContext.QueryString.ContainsKey("id"))
                    {
                        loadWord(int.Parse(NavigationContext.QueryString["id"]));
                    }
                    isSave = true;
                    this.PageTitle.Text = "edit word";
                    this.txtWord.IsReadOnly = true;
                    btnDone.Content = "Save";
                }
                else if (NavigationContext.QueryString["mode"].ToLower() == "new")
                {
                    isSave = false;
                    this.PageTitle.Text = "new word";
                    this.txtWord.IsReadOnly = false;
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
                default_translator.StartTranslate(text, languageFrom, languageTo);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            
            base.OnBackKeyPress(e);
        }

        private void txtWord_TextInput(object sender, TextCompositionEventArgs e)
        {
   
            btnDone.IsEnabled = isEnabled();
            var clean = clearWord(txtWord.Text);
            if (clean.Length > 0)
            {
                doTranslate(clean);
                this.Focus();
            }
        }

        private void txtWord_LostFocus(object sender, RoutedEventArgs e)
        {
            txtTranslations.IsHitTestVisible = true;
        }

        private void txtWord_GotFocus(object sender, RoutedEventArgs e)
        {
            txtTranslations.IsHitTestVisible = false;
        }

        private void txtWord_TextInputStart(object sender, TextCompositionEventArgs e)
        {

        }

        private void txtWord_TextInputUpdate(object sender, TextCompositionEventArgs e)
        {

        }

        private void txtWord_TextChanged(object sender, RoutedEventArgs e)
        {

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
            Dispatcher.BeginInvoke(() =>
                {
                    txtTranslations.Text = "";
                    e.Result.ForEach(s => txtTranslations.Text += s + "\n");
                });
        }
        #endregion

        private void btnSpeak_Click(object sender, RoutedEventArgs e)
        {
            textToSpeech.Speak(txtWord.Text);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            var wordText = clearWord(txtWord.Text);           
            var translations = txtTranslations.Text.Split(new char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries );
            var clear_translations = from string s in translations
                                     where clearWord(s).Length > 0
                                     select clearWord(s);

            Stream audioToStore = textToSpeech.GetAudioStream(txtWord.Text);            

            var selectedVocabularyItem = vocabularyPicker.SelectedItem as Model.VocabularyListItemModel;
            var vocabulary = selectedVocabularyItem == null ? null : selectedVocabularyItem.Vocabulary;

            if (isSave)
            {
                storage.StoreWord(wordId, wordText, clear_translations, vocabulary, audioToStore);                            
            }
            else
            {
                storage.StoreWord(wordText, clear_translations, vocabulary, audioToStore);
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
        #endregion



    }
}