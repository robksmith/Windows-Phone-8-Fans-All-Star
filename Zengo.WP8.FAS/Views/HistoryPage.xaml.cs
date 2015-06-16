
#region Usings

using Microsoft.Phone.Controls;
using Moshen.WP7.FAS.Models;
using System;

#endregion


namespace Moshen.WP7.FAS
{
    public partial class HistoryPage : PhoneApplicationPage
    {
        public HistoryPage()
        {
            InitializeComponent();

            // Set the source
            pitchHistoryLongList.ItemsSource = App.ViewModel.DbViewModel.PitchHistory();

            // Subscribe to change event
            pitchHistoryLongList.SelectionChanged += pitchHistoryLongList_SelectionChanged;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pitchHistoryLongList.SelectedItem = null;
        }

        void pitchHistoryLongList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PitchRecord pitch = pitchHistoryLongList.SelectedItem as PitchRecord;

            if (pitch != null)
            {
                this.NavigationService.Navigate(new Uri("/Views/PitchPage.xaml?Pitch=" + pitch.PitchId, UriKind.Relative));
            }
        }
    }
}