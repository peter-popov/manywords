using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ManyWords.Resources;

namespace ManyWords.Views
{

    #region Binding helpers

    public class StatusToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;

            return b ? 1.0 : 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class StatusToImageConverter : IValueConverter
    {
        private ImageBrush wrong = null;
        private ImageBrush right = null;

        private ImageBrush getWrong()
        {
            if (wrong == null)
            {
                wrong = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/wrong.png", UriKind.Relative)) };
            }
            return wrong;
        }

        private ImageBrush getRight()
        {
            if (right == null)
            {
                right = new ImageBrush { ImageSource = new BitmapImage(new Uri("/Images/right.png", UriKind.Relative)) };
            }
            return right;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;

            return b ? getRight() : getWrong();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
    #endregion

    public partial class AnswerChoiceControl : ExerciseControl
    {
        LocalizedStrings strings = new LocalizedStrings();

        public class AnswerSelectedEventArgs : EventArgs
        {
            public int Index { get; private set; }
            public AnswerSelectedEventArgs(int index)
            {
                this.Index = index;
            }
        }

        public override event EventHandler<EventArgs> AnswerSelected;

        public override void Reset()
        {            
            txtTip.Text = AppResources.AnswerChoice_TapHint;
        }

        public AnswerChoiceControl()
        {
            InitializeComponent();
            rectHidden.Tap += tapAfterSelection;
        }
        
        private void tapAfterSelection(object sender, GestureEventArgs e)
        {           
            if (lstAnswers.SelectedIndex >= 0)
            {
                if ( AnswerSelected != null  )
                    AnswerSelected(this, new EventArgs());

                rectHidden.Visibility = System.Windows.Visibility.Collapsed;
            }

            lstAnswers.SelectedIndex = -1;
        }

        private void lstAnswers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Answer selected, index = {0}", lstAnswers.SelectedIndex);

            //TODO: Yes, it's clumsy, but...
            //Try to signal selected answer to the datacontext
            var answer = this.lstAnswers.SelectedItem as Model.ChoiceAnswer;
            var model = this.DataContext as Model.ChoiceExercise;
            if (answer!= null && model != null)
            {
                model.SubmitAnswer(answer);
            }
            //
            // Fade out unselected elements
            foreach (object item in lstAnswers.Items)
            {
                
                if (e.AddedItems.Contains(item)) continue;

                ListBoxItem listBoxItem = this.lstAnswers.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (listBoxItem != null)
                    VisualStateManager.GoToState(listBoxItem, "Faded", false);
            }

            txtTip.Text = AppResources.AnswerChoice_ContinueHint;

            if (lstAnswers.SelectedIndex >= 0)
                rectHidden.Visibility = System.Windows.Visibility.Visible;            
        }
    }
}
