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

namespace ManyWords.Views
{
    public partial class SettingsView : PhoneApplicationPage
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            languagePicker.ItemsSource = App.LanguagesListModel.MotherLanguages;
            languagePicker.SelectedItem = App.LanguagesListModel.MotherLanguage;
            base.OnNavigatedTo(e);
        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ExtendedLanguageSelect?mode=mother.xaml", UriKind.Relative));
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (languagePicker.SelectedItem != null)
            {
                App.LanguagesListModel.MotherLanguage = languagePicker.SelectedItem as Model.LanguageListItemModel;
            }

            NavigationService.GoBack();
        }

    }
}