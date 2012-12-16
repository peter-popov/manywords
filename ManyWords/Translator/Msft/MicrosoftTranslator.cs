using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel;
using ManyWords.TranslatorService;


namespace ManyWords.Translator.Msft
{
    public class MicrosoftTranslator : ITranslator
    {
        private List<Language> languages = new List<Language>();
        private LanguageServiceClient translator_proxy = new LanguageServiceClient();
        private string APP_ID = "C45B5EADBD4C0FACC60303653AFE04EA151612D2";
        

        public MicrosoftTranslator()
        {

            translator_proxy.TranslateCompleted += translator_TranslateCompleted;
            translator_proxy.SpeakCompleted += translator_SpeakCompleted;
            translator_proxy.GetTranslationsCompleted += translator_TranslateAllCompleted;

            translator_proxy.GetLanguagesForTranslateCompleted +=
                new EventHandler<GetLanguagesForTranslateCompletedEventArgs>
                                     (GetLanguagesForTranslateCompleted);
            translator_proxy.GetLanguageNamesCompleted +=
                       new EventHandler<GetLanguageNamesCompletedEventArgs>
                                                 (GetLanguageNamesCompleted);
        }

        public ICollection<Language> Languages { get { return languages; } }

        public event EventHandler<TranslatedEventArgs<List<string>>> TranslateComplete;
        public event EventHandler<TranslatedEventArgs<Stream>> SpeachReady;

        public void StartTranslate(string text, Language from, Language to, object userState = null)
        {
            //IF we have more than one word use only best translation
            if (text.Split(new char[] { ' ' }).Length > 1)
            {
                translator_proxy.TranslateAsync(APP_ID, text, from.Code, to.Code, "text/plain", "general", userState);
            }
            else
            {
                translator_proxy.GetTranslationsAsync(APP_ID, text, from.Code, to.Code, 4,
                    new TranslateOptions { Category = "general", ContentType = "text/plain" }, userState);
            }
        }

        public void StartSpeach(string text, Language language, object userState = null)
        {
            translator_proxy.SpeakAsync(APP_ID, text, language.Code, "audio/wav", "", userState);
        }

        private void translator_TranslateCompleted(object sender, TranslateCompletedEventArgs e)
        {
            TranslateComplete(this, new TranslatedEventArgs<List<string>>(new List<string> { e.Result }, true, e.UserState));
        }

        private void translator_TranslateAllCompleted(object sender, GetTranslationsCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var translations = e.Result
                                        .Translations.Select(v => v.TranslatedText.Trim())
                                        .Where(v => v != "")
                                        .Distinct(StringComparer.InvariantCultureIgnoreCase);
                TranslateComplete(this, new TranslatedEventArgs<List<string>>(translations.ToList(), true, e.UserState));
            }
            else
            {
                TranslateComplete(this, new TranslatedEventArgs<List<string>>(null, false, e.UserState));
            }
        }


        private void translator_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {

            var client = new WebClient();
            client.OpenReadCompleted += ((s, args) =>
            {
                SpeachReady(this, new TranslatedEventArgs<Stream>(args.Result, true, e.UserState));
            });
            client.OpenReadAsync(new Uri(e.Result));

        }


        private List<string> _codes = new List<string>();
        private List<string> _names = new List<string>();

        void GetLanguagesForTranslateCompleted(object sender,
          GetLanguagesForTranslateCompletedEventArgs e)
        {
            _codes.Clear();
            _codes.AddRange(e.Result);

            translator_proxy.GetLanguageNamesAsync(APP_ID, "en", e.Result);
        }

        void GetLanguageNamesCompleted(object sender,
                 GetLanguageNamesCompletedEventArgs e)
        {
            _names.Clear();
            _names.AddRange(e.Result);

            LoadLanguages();
        }

        private void LoadLanguages()
        {
            for (int index = 0; index < _codes.Count; index++)
            {
                languages.Add(new Language
                {
                    Code = _codes[index],
                    Name = _names[index]
                });
            }
        }

    }
}
