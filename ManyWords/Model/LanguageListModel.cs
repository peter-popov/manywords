using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ManyWords.Translator;
using ManyWords.WordStorage;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace ManyWords.Model
{

    public class LanguageListItemModel : INotifyPropertyChanged
    {
        public string Name { get; private set; }
        public string Code { get; private set; }        
        public bool HasVocabulary { get; set; }


        public LanguageListItemModel(string code, bool hasVocabs)
        {
            this.Code = code;
            this.Name = Translator.TranslatorFactory.LanguageNames[Code];
            this.HasVocabulary = hasVocabs;
        }

        public LanguageListItemModel(string code, string name, bool hasVocabs)
        {
            this.Code = code;
            this.Name = name;
            this.HasVocabulary = hasVocabs;
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

    /// <summary>
    /// 
    /// </summary>
    public class LanguageListModel
    {
        private static string[] basicLanguageList = new string[] { "en", "de", "ru", "es", "it", "fr" };
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
        private Storage storage = App.WordStorage;        

        public LanguageListModel()
        {
            //
            // Get mother languages from db            
            var dbMotherLanguages = (from VocabularyTargetLanguage vtl in storage.wordsDB.TargetLanguages
                                     where vtl.Vocabulary.IsPreloaded
                                     select vtl.Language).Distinct();
         
            MotherLanguages = new ObservableCollection<LanguageListItemModel>();
            foreach (var l in dbMotherLanguages)
                MotherLanguages.Add(new LanguageListItemModel(l, true));

            // Fill list with default languages
            if (MotherLanguages.Count < 5)
            {
                foreach (var l in basicLanguageList)
                    if (MotherLanguages.Count < 5 && !dbMotherLanguages.Contains(l))
                        MotherLanguages.Add(new LanguageListItemModel(l, false));
            }

            // Get from translation engine
            Available = new ObservableCollection<LanguageListItemModel>();
            foreach (var kv in Translator.TranslatorFactory.LanguageNames)
                Available.Add(new LanguageListItemModel(kv.Key, kv.Value, false));

            loadSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LanguageListItemModel> StudyLanguages {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LanguageListItemModel> MotherLanguages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LanguageListItemModel> Available { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private LanguageListItemModel motherLanguage;
        public LanguageListItemModel MotherLanguage
        {
            get
            {
                return motherLanguage;
            }
            set
            {
                if (value != motherLanguage)
                {
                    motherLanguage = MotherLanguages.FirstOrDefault(v => v.Code == value.Code);
                    if (motherLanguage == null)
                    {
                        motherLanguage = value;
                        MotherLanguages.Add(motherLanguage);
                        NotifyPropertyChanged("MotherLanguages");
                    }
                    setting[selected_mother_language_key] = motherLanguage.Code;
                    setting.Save();
                    NotifyPropertyChanged("MotherLanguage");

                    Available.Select(x =>x.HasVocabulary = false);
                    loadStudyLanguages();
                    foreach (var l in StudyLanguages)
                    {
                        var av = Available.FirstOrDefault(v => v.Code == l.Code);
                        if (av != null) av.HasVocabulary = l.HasVocabulary;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private LanguageListItemModel studyLanguage;
        public LanguageListItemModel StudyLanguage
        {
            get
            {
                return studyLanguage;
            }
            set
            {
                if (value.Code == MotherLanguage.Code)
                {
                    throw new ArgumentException("You can not use mother language as study");
                }
                if (value != studyLanguage)
                {
                    studyLanguage = StudyLanguages.FirstOrDefault(v => v.Code == value.Code);
                    if (studyLanguage == null)
                    {
                        studyLanguage = value;
                        StudyLanguages.Add(studyLanguage);
                        NotifyPropertyChanged("StudyLanguages");
                    }
                    setting[selected_study_language_key] = studyLanguage.Code;
                    setting.Save();
                    NotifyPropertyChanged("StudyLanguage");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NeedSelectMotherLanguage { get; private set; }


        #region Save/Load
        private static string selected_mother_language_key = "selected_mother_language";
        private static string selected_study_language_key = "selected_study_language";


        private string getSystemLanguage()
        {
            return "en";
        }


        private void loadStudyLanguages()
        {
            StudyLanguages = new ObservableCollection<LanguageListItemModel>();
            //
            // Get study languages from DB
            var dbStudyLanguages = (from VocabularyTargetLanguage vtl in storage.wordsDB.TargetLanguages
                                    where vtl.Vocabulary.IsPreloaded && vtl.Language == MotherLanguage.Code
                                    select vtl.Vocabulary.Language).Distinct();
            
            foreach (var l in dbStudyLanguages)
            {
                if (l != MotherLanguage.Code && StudyLanguages.Count < 5 )
                    StudyLanguages.Add(new LanguageListItemModel(l, true));                
            }
            
            // Fill list with default languages
            if (StudyLanguages.Count < 5)
            {
                foreach (var l in basicLanguageList)
                    if (l != MotherLanguage.Code && StudyLanguages.Count < 5 && !dbStudyLanguages.Contains(l))
                        StudyLanguages.Add(new LanguageListItemModel(l, false));
            }
            NotifyPropertyChanged("StudyLanguages");
        }


        private void loadSettings()
        {           
            //
            //check stored selected mother language
            var motherLanguageCode = getSystemLanguage();
            if (!setting.Contains(selected_mother_language_key))
            {
                NeedSelectMotherLanguage = true;
            }
            else
            {
                motherLanguageCode = setting[selected_mother_language_key] as string;
                NeedSelectMotherLanguage = false;
            }
            MotherLanguage = Available.FirstOrDefault(v => v.Code == motherLanguageCode);            
            //
            // Retrive last selected study language
            if (!setting.Contains(selected_study_language_key))
            {
                StudyLanguage = StudyLanguages.FirstOrDefault(v => v.Code != motherLanguageCode);
            }
            else
            {
                var code =  setting[selected_study_language_key] as string;
                StudyLanguage = Available.FirstOrDefault(v => v.Code == code);
            }
        }
        #endregion

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
