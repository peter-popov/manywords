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

namespace ManyWords.Views
{
    public partial class PresentWordControl : UserControl
    {
        public PresentWordControl()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> AnswerSelected;

        private void LearnButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: I realy hoped that Command is executed before Click event.
            // It's not true. Have to call it my self, will think how to refactor in future.
            var model = this.DataContext as Model.NewWordExercise;
            if (model != null)
                model.OnLearn.Execute(null);            

            if (AnswerSelected != null)
                AnswerSelected(this, new EventArgs());
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as Model.NewWordExercise;
            if (model != null)
                model.OnSkip.Execute(null);

            if (AnswerSelected != null)
                AnswerSelected(this, new EventArgs());        
        }
    }
}
