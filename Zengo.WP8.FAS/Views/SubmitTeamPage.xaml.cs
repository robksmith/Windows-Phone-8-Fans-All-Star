
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

#endregion


namespace Zengo.WP8.FAS.Views
{
    public partial class SubmitTeamPage : PhoneApplicationPage
    {
        #region Fields

        ObservableCollection<CurrentPitchLocationRecord> pitch;
        ApplicationBarIconButton submitButton;

        TeamSubmitViewModel vm;

        #endregion


        #region Constructors

        public SubmitTeamPage()
        {
            InitializeComponent();

            // Page titles
            PageHeaderControl.PageTitle = AppResources.ProductTitle;
            PageHeaderControl.PageName = "Submit Team";

            // Get the current pitch
            pitch = App.ViewModel.DbViewModel.AllCurrentPitchLocations;

            // Get the view model
            vm = (TeamSubmitViewModel)Resources["ViewModelDataSource"];

            // Subscribe to view model completed event
            vm.Completed += vm_Completed;

            // Build app bar
            BuildApplicationBar();

            TextblockMessage.Text = "Your player selection is listed below. Press the tick at the bottom to submit your two teams";

            // Make sure all 30 positions are filled
            if (pitch.Count < 30)
            {
                TextblockNotEnoughPlayers.Visibility = System.Windows.Visibility.Visible;

                submitButton.IsEnabled = false;

                vm.SetupPlayers(pitch);
                PlayersList.PlayersLongList.ItemsSource = vm.Players;
            }
            else
            {
                TextblockNotEnoughPlayers.Visibility = System.Windows.Visibility.Collapsed;

                submitButton.IsEnabled = true;

                vm.SetupPlayers(pitch);
                PlayersList.PlayersLongList.ItemsSource = vm.Players;
            }
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// They have clicked on the submit tick
        /// </summary>
        void submitButton_Click(object sender, EventArgs e)
        {
            // Disable the button
            submitButton.IsEnabled = false;

            // submit
            vm.SubmitTeam();
        }

        /// <summary>
        /// The response has come back
        /// </summary>
        void vm_Completed(object sender, SubmitTeamEventArgs e)
        {
            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // So the user can't enter the competition again
                App.ViewModel.DbViewModel.UpdateUserEntryAvailable(e.ServerResponse.Response.Available);

                // Send a message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "Your team has been submitted successfully" }, new TimeSpan(0, 0, 3)));

                // Clear the current pitch object
                App.ViewModel.DbViewModel.ClearCurrentPitch();

                // Go home
                NavigationService.GoBack();
            }
            else
            {
                vm.SetupPlayers(pitch);
                PlayersList.PlayersLongList.ItemsSource = vm.Players;
                submitButton.IsEnabled = true;
            }
        }

        #endregion


        #region Helpers

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            submitButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/submit.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarSubmit };
            submitButton.Click += submitButton_Click;
            ApplicationBar.Buttons.Add(submitButton);
        }

        #endregion
    }
}