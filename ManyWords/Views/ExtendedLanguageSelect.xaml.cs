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

        bool useMotherLanguage = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("mode"))
            {
                if (NavigationContext.QueryString["mode"].ToLower() == "study")
                {
                    useMotherLanguage = false;
                }
                else if (NavigationContext.QueryString["mode"].ToLower() == "mother")
                {
                    useMotherLanguage = true;
                }
            }
            base.OnNavigatedTo(e);
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            if ( lstLanguages.SelectedItem != null)
            {
                if ( useMotherLanguage == false )
                    App.LanguagesListModel.StudyLanguage = lstLanguages.SelectedItem as Model.LanguageListItemModel;
                else
                    App.LanguagesListModel.MotherLanguage = lstLanguages.SelectedItem as Model.LanguageListItemModel;
                
                NavigationService.GoBack();
            }            
        }
    }
}