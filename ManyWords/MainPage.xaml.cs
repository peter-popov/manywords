﻿using System;
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

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var vocabularyModel = new Model.VocabularyViewModel();
            lstVocabulary.DataContext = vocabularyModel;
            App.LanguagesListModel.PropertyChanged += vocabularyModel.OnLanguageModelPropertyChanged;            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navugated to main page");
            languagePicker.ItemsSource = App.LanguagesListModel.StudyLanguages;
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ApplicationBarButtonAddWord_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/WordEditor.xaml?mode=new", UriKind.Relative));   
        }

        private void ApplicationBarButtonWordsList_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/WordsView.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MultichoiceTraining.xaml", UriKind.Relative));
        }

        private void languagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (languagePicker.SelectedItem != null)
            {
                App.LanguagesListModel.StudyLanguage = languagePicker.SelectedItem as Model.LanguageListItemModel;
            }
        }

        private void LanguagesListModel_PropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            
        }
    }
}