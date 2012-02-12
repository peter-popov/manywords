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
using System.ComponentModel;
using System.Collections.ObjectModel;



namespace ManyWords.Model
{
    public class TrainingsListItemModel: INotifyPropertyChanged
    {
        public string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }
        

        private string status;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class TrainingsViewModel : INotifyPropertyChanged
    {
        public TrainingsViewModel()
        {
            this.Items = new ObservableCollection<TrainingsListItemModel>();
            LoadData();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<TrainingsListItemModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            this.Items.Add(new TrainingsListItemModel() { Title = "Auto", Status = "mix all types of training" });            
            this.Items.Add(new TrainingsListItemModel() { Title = "DE->EN", Status = "from foreing to native"});
            this.Items.Add(new TrainingsListItemModel() { Title = "EN->DE", Status = "from native to foreing"});
            this.Items.Add(new TrainingsListItemModel() { Title = "Spelling", Status = "check spelling" });            
            this.Items.Add(new TrainingsListItemModel() { Title = "Listening", Status = "translate from voice" });
            
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
