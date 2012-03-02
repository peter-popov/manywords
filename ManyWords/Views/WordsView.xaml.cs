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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ManyWords.Views
{
    public partial class WordsView : PhoneApplicationPage
    {
        private Model.WordsViewModel wordsModel;

        Model.WordListItemModel editedWordItem = null;

        public WordsView()
        {
            InitializeComponent();
            wordsModel = new Model.WordsViewModel();
            DataContext = wordsModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (editedWordItem != null)
            {
                wordsModel.UpdateItem(editedWordItem);
                editedWordItem = null;
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

            wordsModel.Remove(selectedListBoxItem.DataContext as Model.WordListItemModel);
        }
    }
}