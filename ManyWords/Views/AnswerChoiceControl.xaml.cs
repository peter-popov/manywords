﻿using System;
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
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;

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

    public partial class AnswerChoiceControl : UserControl
    {

        public class AnswerSelectedEventArgs : EventArgs
        {
            public int Index { get; private set; }
            public AnswerSelectedEventArgs(int index)
            {
                this.Index = index;
            }
        }

        public event EventHandler<AnswerSelectedEventArgs> AnswerSelected;

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
                    AnswerSelected(this, new AnswerSelectedEventArgs(lstAnswers.SelectedIndex));

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

            if (lstAnswers.SelectedIndex >= 0)
                rectHidden.Visibility = System.Windows.Visibility.Visible;            
        }
    }
}
