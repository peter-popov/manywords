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

namespace ManyWords.Views
{
    public partial class MultichoiceTraining : PhoneApplicationPage
    {
        Model.MultichoiceTrainingModel trainingModel;
        public MultichoiceTraining()
        {
            InitializeComponent();
            
        }



        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }


        object oldDataContext = null;

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            trainingModel = new Model.MultichoiceTrainingModel();
            if (trainingModel.Next())
            {
                oldDataContext = DataContext;
                DataContext = trainingModel;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (!trainingModel.Next())
            {
                DataContext = oldDataContext;
            }
        }
    }
}