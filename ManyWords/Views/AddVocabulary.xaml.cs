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
using ManyWords.WordStorage;
using System.Windows.Navigation;

namespace ManyWords.Views
{
    public partial class AddVocabulary : PhoneApplicationPage
    {
        public AddVocabulary()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            btnReady.IsEnabled = false;
            base.OnNavigatedTo(e);
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            VocabularyTargetLanguage tl = new VocabularyTargetLanguage
            {
                Language = App.LanguagesListModel.MotherLanguage.Code
            };

            Vocabulary v = new Vocabulary
            {
                Description = txtVocabularyName.Text.Trim(),
                Language = App.LanguagesListModel.StudyLanguage.Code
            };

            v.TargetLanguages.Add(tl);

            App.WordStorage.wordsDB.Vocabularies.InsertOnSubmit(v);
            App.WordStorage.wordsDB.TargetLanguages.InsertOnSubmit(tl);
            App.WordStorage.wordsDB.SubmitChanges();
            App.VocabularyListModel.Update();

            NavigationService.GoBack();   
        }

        private void txtVocabularyName_KeyDown(object sender, KeyEventArgs e)
        {
            btnReady.IsEnabled = txtVocabularyName.Text.Trim().Length > 0;
        }
    }
}