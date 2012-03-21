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

namespace ManyWords.Translator
{
    public class TranslatorFactory
    {
        public static readonly Dictionary<string, string> LanguageNames = new Dictionary<string, string>
        {
            {"ar"	 	,"Arabic" },
            {"bg"		,"Bulgarian"},
            {"ca"		,"Catalan"},
            {"cs"		,"Czech"},
            {"da"		,"Danish"},
            {"nl"		,"Dutch"},
            {"en"		,"English"},
            {"et"		,"Estonian"},
            {"fi"		,"Finnish"},
            {"fr"		,"French"},
            {"de"		,"German"},
            {"el"		,"Greek"},
            {"ht"		,"HaitianCreole"},
            {"he"		,"Hebrew"},
            {"hi"		,"Hindi"},
            {"hu"		,"Hungarian"},
            {"id"		,"Indonesian"},
            {"it"		,"Italian"},
            {"ja"		,"Japanese"},
            {"ko"		,"Korean"},
            {"lv"		,"Latvian"},
            {"lt"		,"Lithuanian"},
            {"no"		,"Norwegian"},
            {"pl"		,"Polish"},
            {"pt"		,"Portuguese"},
            {"ro"		,"Romanian"},
            {"ru"		,"Russian"},
            {"sk"		,"Slovak"},
            {"sl"		,"Slovenian"},
            {"es"		,"Spanish"},
            {"sv"		,"Swedish"},
            {"th"		,"Thai"},
            {"tr"		,"Turkish"},
            {"uk"		,"Ukrainian"},
            {"vi"		,"Vietnamese"},
            {"zh-CHS"	,"Chinese(Simplified)"},
            {"zh-CHT"	,"Chinese (Traditional)"}
        };

        public static ITranslator CreateInstance(string options = "")
        {
            return new Msft.MicrosoftTranslator();
        }
    }
}
