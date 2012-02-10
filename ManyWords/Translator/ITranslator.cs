﻿using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ManyWords.Translator
{
    /// <summary>
    /// 
    /// </summary>
    public class Language
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TranslatedEventArgs<T> : EventArgs
    {
        public TranslatedEventArgs(T result, bool ok)
        {
            this.Result = result;
            this.IsOk = ok;
        }

        public bool IsOk { get; private set; }
        public T Result { get; private set; }
    };


    /// <summary>
    /// 
    /// </summary>
    public interface ITranslator
    {
        ICollection<Language> Languages { get; }

        event EventHandler<TranslatedEventArgs<string>> TranslateComplete;
        event EventHandler<TranslatedEventArgs<Stream>> SpeachReady;

        void StartTranslate(string text, Language from, Language to);

        void StartSpeach(string text, Language language);
    }
}
