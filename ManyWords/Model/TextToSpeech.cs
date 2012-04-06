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
using ManyWords.Translator;
using ManyWords.WordStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ManyWords.Model
{
    public class TextToSpeech
    {
        private Language language;
        private ITranslator translator;
        private byte[] audioCache = null;
        private string lastText = null;

        public TextToSpeech(ITranslator translator)
        {
            this.translator = translator;
            this.translator.SpeachReady += translator_SpeakCompleted;
        }

        public TextToSpeech(Language language, ITranslator translator)
        {
            this.language = language;
            this.translator = translator;
            this.translator.SpeachReady += translator_SpeakCompleted;
        }

        private Language CurrentLanguage
        {
            get
            {
                if (language == null)
                    return new Language { Code = App.LanguagesListModel.StudyLanguage.Code };
                return language;
            }
        }

        public Stream GetAudioStream(string text)
        {
            if (lastText != null && clearWord(text) == lastText && audioCache != null)
                return new MemoryStream(audioCache);
            else
                return null;
        }

        public void Speak(string text)
        {
            if (lastText != null && lastText == clearWord(text) && audioCache != null)
            {
                playBuffer(new MemoryStream(audioCache));
            }
            else
            {
                translator.StartSpeach(clearWord(text), CurrentLanguage);
                lastText = clearWord(text);
            }
        }

        public void Speak(Word word)
        {
            if (lastText != null && lastText == word.Spelling && audioCache != null)
            {
                playBuffer(new MemoryStream(audioCache));
                return;
            }

            Stream s = App.WordStorage.GetSpeachAudioStream(word);
            if (s != null)
            {
                playBuffer(s);
            }
            else
            {
                translator.StartSpeach(clearWord(word.Spelling), CurrentLanguage, word);
                lastText = clearWord(word.Spelling);
            }

        }

        private void translator_SpeakCompleted(object sender, TranslatedEventArgs<Stream> e)
        {
            if (e.IsOk)
            {
                cacheSpeech(e.Result);
                playBuffer(new MemoryStream(audioCache));
                var word = e.UserState as Word;
                if (word != null)
                {
                    App.WordStorage.SaveAudio(word, new MemoryStream(audioCache));
                }                
            }
        }

        string clearWord(string s)
        {
            return s.Trim(new char[] { ' ', '.', '\\', '/', '\n' });
        }

        void playBuffer(Stream s)
        {
            FrameworkDispatcher.Update();
            SoundEffect se = SoundEffect.FromStream(s);
            se.Play();
        }

        void cacheSpeech(Stream s)
        {
            audioCache = new byte[s.Length];
            MemoryStream ms = new MemoryStream(audioCache);
            s.CopyTo(ms);
        }


    }
}
