﻿using System;
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
using ManyWords.WordStorage;
using System.Collections.Generic;

namespace ManyWords.Model
{
    public enum ExerciseResult
    {
        OK,
        Wrong,
        Repeat,
        Ignore
    };

    public class Exercise: INotifyPropertyChanged
    {
        public virtual ExerciseResult Result { get; protected set; }

        public virtual ICommand PlaySound { get {return null;} }

        public virtual void Ready() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


}
