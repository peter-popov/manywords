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

    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new ImageBrush { ImageSource = new BitmapImage(new Uri(value.ToString(), UriKind.Relative)) }; ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data           
            //DataContext = App.TrainingsViewModel;

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
    }
}