
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.Views;
using Zengo.WP8.FAS.WepApi;
using Zengo.WP8.FAS.WepApi.Api;
using System;
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class PlayerFindPage : PhoneApplicationPage
    {
        #region Fields

        // simulation stuff
        int testcastcount = 0;
        VoteConfirmEventArgs test;

        // the user api
        UserApi userApi;

        // ascending or descending
        ListSortDirection sortDirection;

        // the search expression
        string searchTerm;

        // a couple of constant 
        const int searchRow = 0;
        const double searchHeight = 70;

        const int feedbackRow = 2;
        const double feedbackHeight = 70;

        // The popup vote control
        Popup popupControl;
        VotePositionSelectControl voteSelectAndConfirmControl;
        VoteConfirmControl voteConfirmControl;
        PlayerStatsControl playerStatsControl;

        #endregion


        #region Constructors

        public PlayerFindPage()
        {
            InitializeComponent();

            // Create the popups
            popupControl = new Popup();

            voteSelectAndConfirmControl = new VotePositionSelectControl();
            voteSelectAndConfirmControl.CancelPressed += voteConfirmControl_CancelPressed;
            voteSelectAndConfirmControl.VotePressed += voteConfirmControl_VotePressed;

            voteConfirmControl = new VoteConfirmControl();
            voteConfirmControl.CancelPressed += voteConfirmControl_CancelPressed;
            voteConfirmControl.VotePressed += voteConfirmControl_VotePressed;

            // Create the player stats popup
            playerStatsControl = new PlayerStatsControl();
            playerStatsControl.BackPressed += playerStatsControl_CancelPressed;

            popupControl.VerticalOffset = 28;

            // Set up the api
            userApi = new UserApi();
            userApi.VoteCompleted += userApi_VoteCompleted;

            // Default search terms
            sortDirection = ListSortDirection.Ascending;
            searchTerm = string.Empty;

            // Do the default search
            PopulateList();

            // We want to know when the user invokes a search
            SearchBox.DoSearch += SearchBox_DoSearch;
            SearchBox.SearchCancelled += SearchBox_SearchCancelled;

            // We want to know when the search feedback has had the clear search button pressed
            SearchBoxFeedback.CancelSearch += SearchBoxResults_CancelSearch;

            // Handles when the pivot changes
            PivotControl.LoadingPivotItem += PivotControl_LoadingPivotItem;

            // Subscribe to the tap event
            //PlayersList.playersLongList.Tap += playersLongList_Tap;
            CountriesList.countriesLongList.Tap += countriesLongList_Tap;
            ClubsList.clubsLongList.Tap += clubsLongList_Tap;
            ZonesList.zonesLongList.Tap += zonesLongList_Tap;

            // Subscribe to the stats and vote tap events
            PlayersList.PlayerStatsTapped += PlayersList_PlayerStatsTapped;
            PlayersList.PlayerVoteTapped += PlayersList_PlayerVoteTapped;

            ByClub.PageName = AppResources.ByClubTitle;
            ByCountry.PageName = AppResources.ByCountryTitle;
            ByName.PageName = AppResources.ByNameTitle;
            ByPosition.PageName = AppResources.ByPositionTitle;
        }

        #endregion


        #region Vote popup events

        void voteConfirmControl_VotePressed(object sender, VoteConfirmEventArgs e)
        {
            // Make sure they are logged on
            if (!App.ViewModel.DbViewModel.IsLoggedOn())
            {
                MessageBox.Show(AppResources.VoteLoginWarning);
                return;
            }

            // If not enough votes left then tell the user and give them the option of visiting the shop
            if (!App.AppConstants.SimulateVote)
            {
                // If not enough votes then tell the user
                if (App.ViewModel.DbViewModel.AvailableVotesCount() <= 0)
                {
                    if (MessageBox.Show(AppResources.NoVotesMessage, AppResources.NoVotesTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        // Go to the buy page
                        NavigationService.Navigate(new Uri("/Views/BuyVotesPage.xaml?removeBackStack=yes", UriKind.Relative));
                    }

                    return;
                }
            }

            // Throw up a warning if they are not validated
            if (!App.ViewModel.DbViewModel.CurrentUser.IsValidated)
            {
                MessageBox.Show(AppResources.ValidateAccountMessage1 + Environment.NewLine + Environment.NewLine + AppResources.ValidateAccountMessage2, AppResources.ValidateAccountTitle, MessageBoxButton.OK);
            }

            // Turn off the popup
            popupControl.IsOpen = false;

            // Turn on the progress bar
            SetProgressIndicator(true);

            // Cast the vote
            if (App.AppConstants.SimulateVote)
            {
                test = e;

                // If simulate vote, just get a list of players to pass the time
                DataApi dataApi = new DataApi();
                dataApi.PlayersCompleted += dataApi_PlayersCompleted;
                dataApi.Players();
            }
            else
            {
                // Get the next available vote and vote
                VoteRecord nextAvailableVote = App.ViewModel.DbViewModel.FindNextAvailableVote();
                if (nextAvailableVote != null)
                {
                    // Mark it as pending in case someone else tries to quickly grab the vote and use it
                    App.ViewModel.DbViewModel.MarkVoteAsPending(nextAvailableVote);

                    // Vote
                    userApi.Vote(App.ViewModel.DbViewModel.CurrentUser.UserId, nextAvailableVote.VoteId, e.Position.PositionId, e.Player.PlayerId);
                }
            }
        }

        void voteConfirmControl_CancelPressed(object sender, EventArgs e)
        {
            popupControl.IsOpen = false;

            ApplicationBar.IsVisible = true;
        }

        void playerStatsControl_CancelPressed(object sender, EventArgs e)
        {
            popupControl.IsOpen = false;

            // show the app bar
            ApplicationBar.IsVisible = true;
        }

        #endregion


        #region Api Events

        void userApi_VoteCompleted(object sender, VoteEventArgs e)
        {
            // Turn off the progress bar
            SetProgressIndicator(false);

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // What player?
                PlayerRecord player = App.ViewModel.DbViewModel.Player(e.ServerResponse.Details.Vote.PlayerId);

                // Add the vote to the db
                App.ViewModel.DbViewModel.VoteSuccessfullyCast(e.ServerResponse.Details);

                // Show a message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = string.Format(AppResources.VoteSuccessful, player.LastName) }, new TimeSpan(0, 0, 2)));

                // Go back
                if (App.ViewModel.DbViewModel.OnPlayerFindPage())
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                // Show a modal failure message
                MessageBox.Show(e.FriendlyMessage, AppResources.VoteFailed, MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// The api has returned - this is just for the simulate vote routine
        /// </summary>
        void dataApi_PlayersCompleted(object sender, PlayersEventArgs e)
        {
            // Turn off the progress bar
            SetProgressIndicator(false);

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                App.ViewModel.DbViewModel.CastSimulatedVote(test.Player.PlayerId, test.Position.PositionId);

                // show a popup success message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = string.Format(AppResources.SimulatedVoteSuccessful, testcastcount++) }, new TimeSpan(0, 0, 2)));
            }
            else
            {
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = string.Format(AppResources.SimulatedVoteFailed, testcastcount++) }, new TimeSpan(0, 0, 2)));
            }
        }

        #endregion


        #region Page and Control Event handlers

        /// <summary>
        /// The players name has been tapped for stats
        /// </summary>
        void PlayersList_PlayerStatsTapped(object sender, Controls.ListItems.PlayerStatsTappedEventArgs e)
        {
            if (e.Player != null)
            {
                // open the popup
                popupControl.Child = playerStatsControl;
                popupControl.IsOpen = true;

                // hide the app bar
                ApplicationBar.IsVisible = false;

                // create the view model
                PlayerStatsViewModel vm = new PlayerStatsViewModel(e.Player.PlayerId);

                playerStatsControl.DataContext = new PlayerListPage.StatsBoundType { Player = e.Player, Stats = vm };
                playerStatsControl.ListBoxStats.DataContext = vm;
                playerStatsControl.TextblockLoading.DataContext = vm;
            }
        }

        /// <summary>
        /// The vote tick has been tapped
        /// </summary>
        void PlayersList_PlayerVoteTapped(object sender, Controls.ListItems.PlayerVoteTappedEventArgs e)
        {
            bool allowClicks = true;

            PlayerRecord player = e.Player;

            if (player != null)
            {
                if (allowClicks)
                {
                    if (!player.IsCulled)
                    {
                        popupControl.Child = voteSelectAndConfirmControl;
                        popupControl.IsOpen = true;

                        ApplicationBar.IsVisible = false;

                        voteSelectAndConfirmControl.Configure(player, null);
                        voteSelectAndConfirmControl.DataContext = new PlayerListPage.VoteBoundType { Player = player, PositionVotedFor = null };
                    }
                }
            }
        }

        /// <summary>
        /// An entry has been tapped
        /// </summary>
        //void playersLongList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    bool allowClicks = true;

        //    if (allowClicks)
        //    {
        //        if (PlayersList.playersLongList.SelectedItem is PlayerRecord)
        //        {
        //            PlayerRecord player = PlayersList.playersLongList.SelectedItem as PlayerRecord;

        //            if (!player.IsCulled)
        //            {
        //                popupControl.Child = voteSelectAndConfirmControl;
        //                popupControl.IsOpen = true;

        //                ApplicationBar.IsVisible = false;

        //                voteSelectAndConfirmControl.Configure(player, null);
        //                voteSelectAndConfirmControl.DataContext = new Zengo.WP8.FAS.PlayerListPage.VoteBoundType { Player = player, PositionVotedFor = null };
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Raised when a user has clicked on a country from the list of countries
        /// </summary>
        void countriesLongList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool allowClicks = true;

            if (allowClicks)
            {
                if (CountriesList.countriesLongList.SelectedItem is CountryRecord)
                {
                    CountryRecord country = CountriesList.countriesLongList.SelectedItem as CountryRecord;

                    string url = string.Format("/Views/PlayerListPage.xaml?country={0}&fromHome={1}", country.CountryId, "False");

                    NavigationService.Navigate(new Uri(url, UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// Raised when a user has clicked on a club from the list of clubs
        /// </summary>
        void clubsLongList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool allowClicks = true;

            if (allowClicks)
            {
                if (ClubsList.clubsLongList.SelectedItem is ClubRecord)
                {
                    ClubRecord club = ClubsList.clubsLongList.SelectedItem as ClubRecord;

                    string url = string.Format("/Views/PlayerListPage.xaml?club={0}&fromHome={1}", club.ClubId, "False");

                    NavigationService.Navigate(new Uri(url, UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// Raised when a user has clicked on a zone from the list of zones
        /// </summary>
        void zonesLongList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool allowClicks = true;

            if (allowClicks)
            {
                if (ZonesList.zonesLongList.SelectedItem is ZoneRecord)
                {
                    ZoneRecord zone = ZonesList.zonesLongList.SelectedItem as ZoneRecord;

                    string url = string.Format("/Views/PlayerListPage.xaml?zone={0}&fromHome={1}", zone.ZoneId, "False");

                    NavigationService.Navigate(new Uri(url, UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// User has invoked a search
        /// </summary>
        void SearchBox_DoSearch(object sender, Controls.SearchBoxControl.DoSearchEventArgs e)
        {
            // Return focus back to screen - get rid of the keyboard
            this.Focus();

            // grab the search term
            searchTerm = e.searchTerm;

            // Do the search / sort
            PopulatePlayersList();

            // Disable search box - do this to close the row
            EnableSearch(false);

            // Populate the info box
            EnableInfo(true, App.ViewModel.DbViewModel.PlayersCount(searchTerm), searchTerm);
        }


        /// <summary>
        /// Clear the search
        /// </summary>
        void SearchBoxResults_CancelSearch(object sender, EventArgs e)
        {
            // Search term is empty
            searchTerm = string.Empty;

            // Populate the list
            PopulatePlayersList();

            // Disable search box
            EnableSearch(false);

            // Disable info box
            EnableInfo(false, 0, string.Empty);
        }

        /// <summary>
        /// When the pivot changes, we want to close any search box already open
        /// </summary>
        //private void PivotControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    EnableSearch(false); 
        //}

        // was the above faster?
        void PivotControl_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            EnableSearch(false); 

            if (e.Item == PlayersPivot)
            {
                ApplicationBar.IsVisible = true;
            }
            else
            {
                ApplicationBar.IsVisible = false;
            }
        }


        /// <summary>
        /// The search box has lost focus meaning the search is cancelled
        /// </summary>
        void SearchBox_SearchCancelled(object sender, EventArgs e)
        {
            EnableSearch(false);
        }

        /// <summary>
        /// Close the popup on back press
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (popupControl.IsOpen)
            {
                ApplicationBar.IsVisible = true;
                popupControl.IsOpen = false;
                e.Cancel = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Turn off the popup here so we dont get flickering if turning off somewhere else and then going to the shop for example
            popupControl.IsOpen = false;
        }

        #endregion


        #region App Bar Events

        /// <summary>
        /// Alternate between sort ascending and descending - NOT done on the find page
        /// </summary>
        //void AppBarMenuItemSortAscending_Click(object sender, EventArgs e)
        //{
        //    ////Toggle the search direction
        //    //sortDirection = sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

        //    //// make sure the app bar menu has the correct text
        //    //UpdateAppBarMenu();

        //    //// Populate the list
        //    //PopulateList();
        //}


        /// <summary>
        /// The search button on the app bar has been pressed
        /// </summary>
        private void AppBarButtonSearch_Click(object sender, System.EventArgs e)
        {
            EnableSearch(!SearchBox.IsSearchEnabled());
        }

        #endregion


        #region Helpers

        internal void EnableSearch(bool enable)
        {
            if (enable)
            {
                GridPlayers.RowDefinitions[searchRow].Height = new GridLength(searchHeight);
            }
            else
            {
                GridPlayers.RowDefinitions[searchRow].Height = new GridLength(0);
                this.Focus();
            }

            SearchBox.EnableSearch(enable);
        }

        private void EnableInfo(bool enable, int results, string searchText)
        {
            if (enable)
            {
                GridPlayers.RowDefinitions[feedbackRow].Height = new GridLength(feedbackHeight);
            }
            else
            {
                GridPlayers.RowDefinitions[feedbackRow].Height = new GridLength(0);
            }

            SearchBoxFeedback.Enable(enable, results, searchTerm);
        }

        //private void UpdateAppBarMenu()
        //{
        //    ApplicationBarMenuItem m = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
        //    m.Text = sortDirection == ListSortDirection.Ascending ? "Sort Descending" : "Sort Ascending";
        //}

        private void PopulateList()
        {
            PopulatePlayersList();

            // At the moment, we cannot list clubs without players because there is too much of a delay!
            //ClubsList.clubsLongList.ItemsSource = App.ViewModel.DbViewModel.ClubsKeyList(false, sortDirection, searchTerm);

            ClubsList.clubsLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.ClubsListForSearch();

            //CountriesList.countriesLongList.ItemsSource = App.ViewModel.DbViewModel.CountriesKeyList(true, sortDirection, searchTerm);
            CountriesList.countriesLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.CountriesListForSearch();

            ZonesList.zonesLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.ZonesKeyList(sortDirection, searchTerm);
        }

        private void PopulatePlayersList()
        {
            PlayersList.PlayersLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.PlayersList(sortDirection, searchTerm);
        }

        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = AppResources.CastingVote
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion
    }
}