using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ManyWords.TranslatorService;


namespace ManyWords.Translator.Msft
{
    public class MicrosoftTranslator: ITranslator
    {
        private List<Language> languages = new List<Language>();
        private LanguageServiceClient translator_proxy = new LanguageServiceClient();
        private string APP_ID = "C45B5EADBD4C0FACC60303653AFE04EA151612D2";

        public MicrosoftTranslator()
        {
            languages.Add( new Language{ Code="en", Name="English" } );
            languages.Add( new Language{ Code="de", Name="German" } );
            languages.Add( new Language{ Code="ru", Name="Russian" } );

            translator_proxy.TranslateCompleted += translator_TranslateCompleted;
            translator_proxy.SpeakCompleted += translator_SpeakCompleted ;
            translator_proxy.GetTranslationsCompleted += translator_TranslateAllCompleted;
        }

        public ICollection<Language> Languages { get { return languages; } }

        public event EventHandler<TranslatedEventArgs<List<string>>> TranslateComplete;
        public event EventHandler<TranslatedEventArgs<Stream>> SpeachReady;

        public void StartTranslate(string text, Language from, Language to)
        {            
            //IF we have more than one word use only best translation
            if (text.Split(new char[] { ' ' }).Length > 1)
            {
                translator_proxy.TranslateAsync(APP_ID, text, from.Code, to.Code, "text/plain", "general");
            }
            else
            {
                translator_proxy.GetTranslationsAsync(APP_ID, text, from.Code, to.Code, 4,
                    new TranslateOptions { Category = "general", ContentType = "text/plain" });
            }
        }

        public void StartSpeach(string text, Language language)
        {
            translator_proxy.SpeakAsync(APP_ID, text, language.Code, "audio/wav", "");
        }

        private void translator_TranslateCompleted(object sender, TranslateCompletedEventArgs e)
        {
            TranslateComplete(this, new TranslatedEventArgs<List<string>>(new List<string>{ e.Result }, true));
        }

        private void translator_TranslateAllCompleted(object sender, GetTranslationsCompletedEventArgs e)
        {
            List<string> list = new List<string>();
            
            foreach (TranslationMatch match in e.Result.Translations)
            {
                if (match.TranslatedText.Trim() == "")
                    continue;

                if (!list.Contains(match.TranslatedText))
                    list.Add(match.TranslatedText);
            }

            TranslateComplete(this, new TranslatedEventArgs<List<string>>(list, true));
        }


        private void translator_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            var client = new WebClient();
            client.OpenReadCompleted += ((s, args) =>
            {
                SpeachReady(this, new TranslatedEventArgs<Stream>(args.Result, true));
            });
            client.OpenReadAsync(new Uri(e.Result));
        }
    }
}
