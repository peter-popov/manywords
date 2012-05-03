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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ManyWords.Utils;

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
            VisualStateManager.GoToState(iconResult, "Normal", false);   
            this.Focus();
            txtInput.Focus();
            txtTip.Text = "Enter translation for the word above";
            txtInput.Text = "";
            btnCheck.Visibility = Visibility.Visible;
            panelAnswer.Visibility = Visibility.Collapsed;
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
            btnCheck.Focus();
            //TODO: Yes, it's clumsy, but...
            //Try to signal selected answer to the datacontext            
            var model = this.DataContext as Model.SpellingExercise;
            if (model != null)
            {
                model.SubmitAnswer(txtInput.Text);
            }

            if (model.Result == Model.ExerciseResult.OK)
            {
                VisualStateManager.GoToState(iconResult, "Correct", true); 
            }
            else
            {
                panelAnswer.Visibility = System.Windows.Visibility.Visible;
                //
                // Show expected result and highlight problematic parts
                showDiff(txtInput.Text.Trim(), model.Word.Trim());
                VisualStateManager.GoToState(iconResult, "Wrong", true); 
            }

            txtTip.Text = "Tap somewhere to continue";
            rectHidden.Visibility = System.Windows.Visibility.Visible;
            btnCheck.Visibility = Visibility.Collapsed;
        }

        private void showCorrect(string text, bool correct = true)
        {
            txtAnswer.Blocks.Clear();
            Paragraph p1 = new Paragraph();
            {
                Run r = new Run();
                r.FontSize = txtInput.FontSize;
                r.Text = text;
                r.Foreground = new SolidColorBrush(Colors.Black);
                p1.Inlines.Add(r);
            }
            txtAnswer.Blocks.Add(p1);  
        }

        private void showDiff(string text1, string text2)
        {
            var result = DiffAlg.Diff(
                text1.ToCharArray(), 0, text1.Length,
                text2.ToCharArray(), 0, text2.Length,
                SpecialEqualityComparer.Default);

            txtAnswer.Blocks.Clear();

            int distance = result.Where(x => x.Type != DiffAlg.DiffSectionType.Copy).Select(x => x.Length).Sum();

            if (distance > 4)
            {
                showCorrect(text2, false);
                return;
            }

            Paragraph p1 = new Paragraph();
            Paragraph p2 = new Paragraph();

            int pos1 = 0, pos2 = 0;
            string word = "";
            string mask = "";
            foreach (var d in result)
            {
                Run r = new Run();
                r.FontSize = txtInput.FontSize;
                if (d.Type == DiffAlg.DiffSectionType.Copy)
                {
                    r.Text = text2.Substring(pos1, d.Length);
                    r.Foreground = txtAnswer.Foreground;

                    pos1 += d.Length;
                    pos2 += d.Length;
                    p1.Inlines.Add(r);
                    p2.Inlines.Add(new Run { Text = r.Text, FontSize = 30 });
                }
                else if (d.Type == DiffAlg.DiffSectionType.Insert)
                {
                    r.Text += text2.Substring(pos1, d.Length);
                    r.Foreground = new SolidColorBrush(Colors.Green);

                    pos1 += d.Length;
                    p2.Inlines.Add(r);
                }
                else if (d.Type == DiffAlg.DiffSectionType.Delete)
                {
                    r.Text += text1.Substring(pos2, d.Length);
                    r.Foreground = new SolidColorBrush(Colors.Red);

                    pos2 += d.Length;
                    p1.Inlines.Add(r);
                }

            }

            //txtAnswer.Blocks.Add(p1);
            txtAnswer.Blocks.Add(p2);


            System.Diagnostics.Debug.WriteLine(word);
            System.Diagnostics.Debug.WriteLine(mask);

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
