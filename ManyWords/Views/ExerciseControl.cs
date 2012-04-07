using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ManyWords.Views
{
    public class ExerciseControl: UserControl
    {
        public virtual event EventHandler<EventArgs> AnswerSelected;

        public virtual void Reset()
        {            
        }
    }
}
