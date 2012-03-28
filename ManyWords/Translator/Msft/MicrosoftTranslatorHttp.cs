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
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ManyWords.Translator.Msft
{
    public class MicrosoftTranslatorHttp : ITranslator
    {
        private List<Language> languages = new List<Language>();
       
        private string APP_ID = "C45B5EADBD4C0FACC60303653AFE04EA151612D2";

        private string getTranslateMethod(string from, string to, string text)
        {
            return string.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?appId={0}&text={1}&from={2}&to={3}",
                APP_ID, text, from, to);
        }

        private string getSpeakMethod(string language, string text)
        {
            return string.Format("http://api.microsofttranslator.com/v2/Http.svc/Speak?appId={0}&language={1}&text={2}",
                APP_ID, language, text);
        }

        public ICollection<Language> Languages { get { return new List<Language>(); } }


        public event EventHandler<TranslatedEventArgs<List<string>>> TranslateComplete;
        public event EventHandler<TranslatedEventArgs<Stream>> SpeachReady;

        public void StartTranslate(string text, Language from, Language to, object userState = null)
        {
            var uriRequest = getTranslateMethod(from.Code, to.Code, text);
            // The signature for SendStringRequest looks like this:
            //
            //  public static RequestInfo 
            //      SendStringRequest(
            //          string uriString,
            //          Action sent,
            //          Action<string> received,
            //          Action<string> failed );
            //
            WebRequestHelper.RequestInfo returnValue =
                    WebRequestHelper.SendStringRequest(
                        uriRequest,
                        () => // Sent()
                        {
                            // Do nothing on sent                            
                        },
                        (resultXML) => // Received(string resultXML)
                        {
                            // This code is called from the WebRequest's thread, so anything that touches the UI
                            // will need to be marshalled
                            string translatedText = ParseResult(resultXML);

                            if (string.IsNullOrEmpty(translatedText))
                            {                 
                                TranslateComplete(this, new TranslatedEventArgs<List<string>>(null, false, userState));
                            }
                            else
                            {                               
                                TranslateComplete(this, new TranslatedEventArgs<List<string>>(new List<string>() { translatedText }, 
                                                                                                                   true, 
                                                                                                                   userState ));                               
                            }

                        },
                        (errorMsg) => // Failed(string errorMsg)
                        {                          
                            System.Diagnostics.Debug.WriteLine(errorMsg);
                            TranslateComplete(this, new TranslatedEventArgs<List<string>>(null, false, userState));
                        }
                    );
        }

        public void StartSpeach(string text, Language language, object userState = null)
        {
            var uriRequest = getSpeakMethod(language.Code, text);
            // The signature of WebRequestHelper.SendByteRequest is
            //
            //      public static RequestInfo SendByteRequest(
            //          string uriString,
            //          Action sent,
            //          Action<byte[]> received,
            //          Action<string> failed
            //
            // So we are able to put all the actions for sent/received/failed 
            // down in this one section of code.
            WebRequestHelper.SendByteRequest(
                                uriRequest,
                                () => // Sent()
                                {
                                    //Do nothing
                                },
                                (resultBytes) => // Received(byte[] resultBytes)
                                {
                                    if (SpeachReady != null)
                                    {
                                        SpeachReady(this, new TranslatedEventArgs<Stream>(new MemoryStream(resultBytes), true, userState));
                                    }                                       
                                },
                                (errorMsg) => // Failed(string errorMsg)
                                {
                                    if (SpeachReady != null)
                                    {
                                        SpeachReady(this, new TranslatedEventArgs<Stream>(null, false, userState));
                                    }    
                                }
                            );
        }


        /// <summary>
        /// Parses the XML into human readable format.
        /// Input format looks like this:
        /// 
        /// <string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Bonjour</string>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>The translated text in human-readable form</returns>
        private string ParseResult(string xml)
        {
            string returnValue = string.Empty;

            if (string.IsNullOrEmpty(xml) == false)
            {
                try
                {
                    using (StringReader stringReader = new StringReader(xml))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(stringReader))
                        {
                            xmlReader.ReadStartElement();
                            if (xmlReader.ValueType.Name == "String")
                            {
                                returnValue = xmlReader.Value;
                            }
                        }
                    }
                }
                catch (XmlException)
                {
                    // Any XML parse errors means we let the default null return, which
                    // will put up a friendly error message
                }
            }

            // White space often comes back in the translations, lose that.
            return returnValue.Trim();
        }
    }
}
