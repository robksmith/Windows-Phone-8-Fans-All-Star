using System;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using Newtonsoft.Json;

namespace Zengo.WP8.FAS
{
    public partial class FreeEntryPage
    {
        #region Fields

        ApplicationBarIconButton freeEntryButton;

        #endregion

        #region Constructors

        public FreeEntryPage()
        {
            InitializeComponent();

            PageHeaderControl.PageName = AppResources.FreeEntry;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;

            FreeEntryControl.FavouritePlayerPressed += FreeEntryControlOnFavouritePlayerPressed;
            FreeEntryControl.FavouriteLeaguePressed += FreeEntryControlOnFavouriteLeaguePressed;
            FreeEntryControl.FavouriteStadiumPressed += FreeEntryControlOnFavouriteStadiumPressed;
            FreeEntryControl.FavouriteSportPressed += FreeEntryControlOnFavouriteSportPressed;
            FreeEntryControl.FreeEntryStarting += FreeEntryControlOnFreeEntryStarting;
            FreeEntryControl.FreeEntryCompleted += FreeEntryControlOnFreeEntryCompleted;
            BuildApplicationBar();

            // Get our register button so we can enable/disable it
            freeEntryButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            FreeEntryControl.Page = this;

            BuildApplicationBar();

            freeEntryButton = (ApplicationBarIconButton) ApplicationBar.Buttons[0];
        }

       private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarSubmit };
            button.Click += AppBarMenuEnter_Click;
            ApplicationBar.Buttons.Add(button);

        }


        #endregion

        #region Page Event Handlers

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            FreeEntryControl.SetFavourites(App.AppConstants.FavPlayer, App.AppConstants.FavLeague, App.AppConstants.FavStadium, App.AppConstants.FavSport);

            FreeEntryControl.ResetErrors();
        }

        #endregion

        private void FreeEntryControlOnFavouritePlayerPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouritePlayerPage.xaml", UriKind.Relative));
        }

        private void FreeEntryControlOnFavouriteLeaguePressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteLeaguePage.xaml", UriKind.Relative));
        }

        private void FreeEntryControlOnFavouriteSportPressed(object sender, EventArgs eventArgs)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteSportPage.xaml", UriKind.Relative));

        }

        private void FreeEntryControlOnFavouriteStadiumPressed(object sender, EventArgs eventArgs)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteStadiumPage.xaml", UriKind.Relative));
        }

        private void FreeEntryControlOnFreeEntryCompleted(object sender, FreeEntryControl.FreeEntryCompletedEventArgs e)
        {
            if (e.Success)
            {
                MessageBox.Show("Your free votes have been credited to your account", "Free votes", MessageBoxButton.OK);

                if (App.ViewModel.DbViewModel.OnFreeEntryPage())
                {
                    NavigationService.RemoveBackEntry();
                    NavigationService.GoBack();
                }
            }
        }

        private void FreeEntryControlOnFreeEntryStarting(object sender, EventArgs e)
        {
            // Disable the button
            freeEntryButton.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // turn on the progress bar
            SetProgressIndicator(true);
        }

        #region App Bar Events

        private void AppBarMenuEnter_Click(object sender, EventArgs e)
        {
            FreeEntryControl.FreeEntry();
        }

        #endregion


        #region Helpers

        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = AppResources.Submitting
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion

    }
}