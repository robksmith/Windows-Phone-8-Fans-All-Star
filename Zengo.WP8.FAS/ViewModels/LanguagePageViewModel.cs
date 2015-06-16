using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Zengo.WP8.FAS.Helpers;

namespace Zengo.WP8.FAS.ViewModels
{
    public class LanguagePageViewModel : LanguageSettingsViewModel
    {
        public LanguagePageViewModel()
        {
            AddLanguages(new Language { Description = "Français", Locale = "fr", FlagUri = new Uri("../Images/Flags/50f34779-2dd0-4af1-ad3c-7e8f0a300d19@2x.png", UriKind.RelativeOrAbsolute) });
            AddLanguages(new Language { Description = "Deutch", Locale = "de", FlagUri = new Uri("../Images/Flags/50f3f624-4b9c-4b31-be29-11490a300d19@2x.png", UriKind.RelativeOrAbsolute) });
            AddLanguages(new Language { Description = "Italiano", Locale = "it", FlagUri = new Uri("../Images/Flags/50f346f4-c5d0-4455-8ec7-0ea40a300d19@2x.png", UriKind.RelativeOrAbsolute) });
            AddLanguages(new Language { Description = "Português", Locale = "pt", FlagUri = new Uri("../Images/Flags/50f4649a-99d4-4bef-89df-7e900a300d19@2x.png", UriKind.RelativeOrAbsolute) });
            AddLanguages(new Language { Description = "Español", Locale = "es", FlagUri = new Uri("../Images/Flags/50f463e4-7088-4d87-a492-7e8c0a300d19@2x.png", UriKind.RelativeOrAbsolute) });

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLanguage")
            {
                SetLanguageFromCurrentLocate();
                LocalizedStrings.LocalizedStringsResource.UpdateLanguage();
            }
        }
    }
}
