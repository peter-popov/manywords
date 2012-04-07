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
    public partial class SpellingTrainingControll : ExerciseControl
    {
        public SpellingTrainingControll()
        {
            InitializeComponent();
            rectHidden.Tap += tapAfterSelection;
        }

        public override void Reset()
        {
            this.Focus();
            txtInput.Focus();
            txtTip.Text = "Enter translation for the word above";
            txtInput.Text = "";
            btnCheck.Visibility = Visibility.Visible;
        }

        public override event EventHandler<EventArgs> AnswerSelected;

        private void tapAfterSelection(object sender, GestureEventArgs e)
        {
            if (AnswerSelected != null)
                AnswerSelected(this, new EventArgs());
            rectHidden.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void txtInput_TextInput(object sender, TextCompositionEventArgs e)
        {
            //this.Focus();
            btnCheck.Focus();
            //TODO: Yes, it's clumsy, but...
            //Try to signal selected answer to the datacontext            
            var model = this.DataContext as Model.SpellingExercise;
            if (model != null)
            {
                model.SubmitAnswer(txtInput.Text);
            }
            txtTip.Text = "Tap somewhere to continue";
            rectHidden.Visibility = System.Windows.Visibility.Visible;
            btnCheck.Visibility = Visibility.Collapsed;
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Yes, it's clumsy, but...
            //Try to signal selected answer to the datacontext
            var model = this.DataContext as Model.SpellingExercise;
            if (model != null)
            {
                model.SubmitAnswer(txtInput.Text);
            }
            txtTip.Text = "Tap somewhere to continue";
            rectHidden.Visibility = System.Windows.Visibility.Visible;
            btnCheck.Visibility = Visibility.Collapsed;
        }
    }
}
