using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace ManyWords.Views
{
    public partial class ExtendedLanguageSelect : PhoneApplicationPage
    {
        public ExtendedLanguageSelect()
        {
            InitializeComponent();
            lstLanguages.ItemsSource = App.LanguagesListModel.Available;
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            if ( lstLanguages.SelectedItem != null)
            {
                App.LanguagesListModel.StudyLanguage = lstLanguages.SelectedItem as Model.LanguageListItemModel;
                NavigationService.GoBack();
            }            
        }
    }
}