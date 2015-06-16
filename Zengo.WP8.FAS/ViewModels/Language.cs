using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Zengo.WP8.FAS.ViewModels
{
    public class Language : INotifyPropertyChanged, IEquatable<Language>
    {
        private string locale;
        public string Locale
        {
            get { return locale; }
            set
            {
                if (locale != value)
                {
                    locale = value;
                    OnPropertyChanged("Locale");
                }
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        private Uri flagUri;
        public Uri FlagUri
        {
            get { return flagUri; }
            set
            {
                if (flagUri != value)
                {
                    flagUri = value;
                    OnPropertyChanged("FlagUri");
                }
            }
        }

        public override string ToString()
        {
            return Description;
        }

        public bool Equals(Language other)
        {
            return other != null && other.Locale.Equals(Locale);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Language);
        }

        public override int GetHashCode()
        {
            return Locale.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
