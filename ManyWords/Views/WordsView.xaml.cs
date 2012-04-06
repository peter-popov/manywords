using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Controls;

namespace ManyWords.Views
{

    
    public partial class WordsView : PhoneApplicationPage
    {
        private Model.WordsViewModel wordsModel;

        Model.WordListItemModel editedWordItem = null;

        public WordsView()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (editedWordItem != null)
            {
                wordsModel.UpdateItem(editedWordItem);
                editedWordItem = null;
            }
            else
            {
                WordStorage.Vocabulary useVocabulary = null; 
                if (NavigationContext.QueryString.ContainsKey("vocabulary"))
                {
                    int id = int.Parse( NavigationContext.QueryString["vocabulary"] );
                    useVocabulary = App.WordStorage.wordsDB.Vocabularies.Where(x => x.ID == id).FirstOrDefault();
                }
                wordsModel = useVocabulary == null ? new Model.WordsViewModel(App.TextToSpeech) : new Model.WordsViewModel(App.TextToSpeech, useVocabulary);
                DataContext = wordsModel;
            }
        }

        private void Edit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem selectedListBoxItem = this.allList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
            if (selectedListBoxItem == null)
            {
                return;
            }

            Model.WordListItemModel wordItem = selectedListBoxItem.DataContext as Model.WordListItemModel;

            if (wordItem != null)
            {
                var url = string.Format("/Views/WordEditor.xaml?mode=edit&id={0}", wordItem.Word.WordID);
                NavigationService.Navigate(new Uri(url, UriKind.Relative));

                editedWordItem = wordItem;
            }            
        }

        private void Delete_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedListBoxItem = this.allList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem;
            if (selectedListBoxItem == null)
            {
                return;
            }

            var wordItem = selectedListBoxItem.DataContext as Model.WordListItemModel;

            var res = MessageBox.Show("Are you sure you want to completly remove this word ?",
                                       "Remove \"" + wordItem.Spelling + "\"",
                                       MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                wordsModel.Remove(wordItem);
            }
        }

        private void txtSearch_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void txtSearch_TextInputUpdate(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (wordsModel != null)
                wordsModel.Filter(txtSearch.Text);
        }
    }


    public class ProgressToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = value as WordStorage.Word;

            if (w != null && w.Level > 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class ProgressToWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var w = value as WordStorage.Word;

            if (w != null && w.Level > 0)
            {
                return w.Level;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }


}