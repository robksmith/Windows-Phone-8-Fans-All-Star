
#region Usings

using System.ComponentModel;
using System.Windows;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class LocalizedStrings : INotifyPropertyChanged
    {
        private static AppResources _localizedresources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedresources; } }

        public void UpdateLanguage()
        {
            _localizedresources = new AppResources();
            OnPropertyChanged("LocalizedResources");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public static LocalizedStrings LocalizedStringsResource { get { return Application.Current.Resources["LocalizedStrings"] as LocalizedStrings; } }
    }
}
