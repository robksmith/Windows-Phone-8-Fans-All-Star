using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;

namespace Zengo.WP8.FAS.ViewModels
{
    public class LanguageSettingsViewModel : INotifyPropertyChanged
    {
        public LanguageSettingsViewModel()
        {
            SupportedLanguages = new ObservableCollection<Language>();
            AddLanguages(new Language { Description = "English", Locale = "en", FlagUri = new Uri("../Images/Flags/5135fee9-6b58-402e-b4d2-777f0a300d19@2x.jpeg", UriKind.RelativeOrAbsolute) });
        }

        private ObservableCollection<Language> supportedLanguages;
        public ObservableCollection<Language> SupportedLanguages
        {
            get { return supportedLanguages; }
            set
            {
                if (supportedLanguages != value)
                {
                    supportedLanguages = value;
                    OnPropertyChanged("SupportedLanguages");

                }
            }
        }

        private Language GetDefaultLanguage()
        {
            var storage = IsolatedStorageSettings.ApplicationSettings;

            
            var lang =  
                SupportedLanguages.FirstOrDefault(p => p.Locale == (string)storage["language"]) ??
                SupportedLanguages.FirstOrDefault(p => p.Locale == Thread.CurrentThread.CurrentUICulture.Name) ??
                SupportedLanguages.FirstOrDefault(p => p.Locale.StartsWith(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName)) ??
                SupportedLanguages.First(p => p.Locale.Contains("en"));

            return lang;
        }

        private Language currentLanguage;
        public Language CurrentLanguage
        {
            get
            {
                return currentLanguage;
            }
            set
            {
                if (!Equals(currentLanguage, value))
                {
                    currentLanguage = value;
                    OnPropertyChanged("CurrentLanguage");
                }
            }
        }

        public void SetLanguageFromCurrentLocate()
        {
            if (CurrentLanguage == null)
            {
                CurrentLanguage = GetDefaultLanguage();
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(CurrentLanguage.Locale);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CurrentLanguage.Locale);
            var storage = IsolatedStorageSettings.ApplicationSettings;
            storage["language"] = currentLanguage.Locale;
            storage.Save();
        }

        public void AddLanguages(params Language[] languages)
        {
            foreach (var l in languages)
            {
                if (!supportedLanguages.Contains(l))
                {
                    supportedLanguages.Add(l);
                }
            }    
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
