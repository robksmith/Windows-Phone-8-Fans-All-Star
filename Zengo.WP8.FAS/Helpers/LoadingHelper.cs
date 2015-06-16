
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class LoadingHelper : INotifyPropertyChanged
    {
        #region Fields

        private ProgressIndicator progressIndicator;

        #endregion


        #region Constructors

        private LoadingHelper()
        {
        }

        #endregion


        public void Initialize(PhoneApplicationFrame frame)
        {
            // If using AgFx:
            // DataManager.Current.PropertyChanged += OnDataManagerPropertyChanged;

            progressIndicator = new ProgressIndicator();

            frame.Navigated += OnRootFrameNavigated;
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.
            var ee = e.Content;
            var pp = ee as PhoneApplicationPage;
            if (pp != null)
            {
                pp.SetValue(SystemTray.ProgressIndicatorProperty, progressIndicator);
            }
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                // if AgFx: IsDataManagerLoading = DataManager.Current.IsLoading;
                NotifyValueChanged();
            }
        }

        private static LoadingHelper instance;
        public static LoadingHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoadingHelper();
                }

                return instance;
            }
        }

        public bool IsDataManagerLoading { get; set; }

        public bool ActualIsLoading
        {
            get
            {
                return IsLoading || IsDataManagerLoading;
            }
        }

        private int _loadingCount;

        public bool IsLoading
        {
            get
            {
                return _loadingCount > 0;
            }
            set
            {
                bool loading = IsLoading;
                if (value)
                {
                    ++_loadingCount;
                }
                else
                {
                    --_loadingCount;
                }

                NotifyValueChanged();
            }
        }

        private void NotifyValueChanged()
        {
            if (progressIndicator != null)
            {
                progressIndicator.IsIndeterminate = _loadingCount > 0 || IsDataManagerLoading;

                // for now, just make sure it's always visible.
                if (progressIndicator.IsVisible == false)
                {
                    progressIndicator.IsVisible = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
