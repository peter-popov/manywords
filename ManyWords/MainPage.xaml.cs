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
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using ManyWords.Translator;

namespace ManyWords
{

    public partial class MainPage : PhoneApplicationPage
    {
        bool processLanguageSelect = false;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            lstVocabulary.DataContext = App.VocabularyListModel;
            App.LanguagesListModel.PropertyChanged += App.VocabularyListModel.OnLanguageModelPropertyChanged;            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navigated to main page");

            languagePicker.ItemsSource = App.LanguagesListModel.StudyLanguages;
            languagePicker.SelectedItem = App.LanguagesListModel.StudyLanguage;
            processLanguageSelect = true;
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ApplicationBarButtonAddWord_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/WordEditor.xaml?mode=new", UriKind.Relative));   
        }

        private void AddWord_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/WordsView.xaml", UriKind.Relative));
        }

        private void StartTraining_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/TrainingView.xaml", UriKind.Relative));
        }

        private void languagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languagePicker.SelectedItem != null && processLanguageSelect)
            {
                App.LanguagesListModel.StudyLanguage = languagePicker.SelectedItem as Model.LanguageListItemModel;
            }
        }

        private void LanguagesListModel_PropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            
        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ExtendedLanguageSelect.xaml", UriKind.Relative));
        }

        private void lstVocabulary_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedItem = lstVocabulary.SelectedItem as Model.VocabularyListItemModel;
            if (selectedItem != null)
            {
                NavigationService.Navigate(new Uri(string.Format("/Views/WordsView.xaml?vocabulary={0}", selectedItem.Vocabulary.ID), UriKind.Relative));
                lstVocabulary.SelectedItem = null;
            }
        }

        private void btnAddVocabulary_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AddVocabulary.xaml", UriKind.Relative));
        }

        private void DeleteVocabulary_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var selectedItem = (sender as MenuItem).DataContext as Model.VocabularyListItemModel;
                if (selectedItem != null)
                {
                    App.WordStorage.RemoveVocabulary(selectedItem.Vocabulary);
                }
            }
        }
    }
}