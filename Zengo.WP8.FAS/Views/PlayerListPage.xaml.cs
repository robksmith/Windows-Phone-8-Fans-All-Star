
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.WepApi;
using Zengo.WP8.FAS.WepApi.Api;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class PlayerListPage : PhoneApplicationPage
    {
        #region Inner Classes

        public class VoteBoundType
        {
            public PlayerRecord Player { get; set; }
            public PositionRecord PositionVotedFor { get; set; }
        }

        public class StatsBoundType
        {
            public PlayerRecord Player { get; set; }
            public PlayerStatsViewModel Stats { get; set; }
        }

        #endregion


        #region Fields

        // simulation stuff
        int testcastcount = 0;
        VoteConfirmEventArgs test;

        // the user api
        UserApi userApi;
        bool allowClicks = false;

        // Which popup do we want - if true, the player can alter his selected position
        bool letUserChangeSelectedPosition = true;

        // The two popups posible from this page
        Popup popupControl;
        VotePositionSelectControl voteSelectAndConfirmControl;
        VoteConfirmControl voteConfirmControl;
        PlayerStatsControl playerStatsControl;

        // for display player by zone - picked up from the query string
        ZoneRecord zonePlayerIsIn = null;

        // for display player by club - picked up from the query string
        ClubRecord clubPlayerIsIn;

        // for display player by country - picked up from the query string
        CountryRecord countryPlayerIsIn;

        // for when a user already know what position they want players to be from - picked up from the query string
        PositionRecord positionVotedFor = null;
        private bool fromHome;

        #endregion


        #region Constructors

        public PlayerListPage()
        {
            InitializeComponent();

            // Create the popups
            popupControl = new Popup();

            // Create the vote popup for players
            voteSelectAndConfirmControl = new VotePositionSelectControl();
            voteSelectAndConfirmControl.CancelPressed += voteConfirmControl_CancelPressed;
            voteSelectAndConfirmControl.VotePressed += voteConfirmControl_VotePressed;

            // Create the vote popup for managers
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

            // Subscribe to the stats and vote tap events
            PlayersList.PlayerStatsTapped += PlayersList_PlayerStatsTapped;
            PlayersList.PlayerVoteTapped += PlayersList_PlayerVoteTapped;

            // Fill inthe titles
            PageHeaderControl.PageTitle = AppResources.ProductTitle;
            PageHeaderControl.PageTitle = AppResources.PlayersTitle;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// The vote tick has been tapped
        /// </summary>
        void PlayersList_PlayerVoteTapped(object sender, Controls.ListItems.PlayerVoteTappedEventArgs e)
        {
            PlayerRecord player = e.Player;

            if (player != null)
            {
                if (allowClicks)
                {
                    if (!player.IsCulled)
                    {
                        bool manager = false;

                        if (player.Zone.Name == PitchConstants.ZoneManager)
                        {
                            manager = true;
                        }

                        if (letUserChangeSelectedPosition && !manager)
                        {
                            popupControl.Child = voteSelectAndConfirmControl;
                            popupControl.IsOpen = true;

                            voteSelectAndConfirmControl.Configure(player, positionVotedFor);
                            voteSelectAndConfirmControl.DataContext = new VoteBoundType { Player = player, PositionVotedFor = positionVotedFor };
                        }
                        else
                        {
                            // The manager definitely has to use this one
                            popupControl.Child = voteConfirmControl;
                            popupControl.IsOpen = true;

                            voteConfirmControl.DataContext = new VoteBoundType { Player = player, PositionVotedFor = positionVotedFor };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The players name has been tapped for stats
        /// </summary>
        void PlayersList_PlayerStatsTapped(object sender, Controls.ListItems.PlayerStatsTappedEventArgs e)
        {
            if (e.Player != null)
            {
                popupControl.Child = playerStatsControl;
                popupControl.IsOpen = true;

                PlayerStatsViewModel vm = new PlayerStatsViewModel(e.Player.PlayerId);

                playerStatsControl.DataContext = new StatsBoundType { Player = e.Player, Stats = vm };
                playerStatsControl.ListBoxStats.DataContext = vm;
                playerStatsControl.TextblockLoading.DataContext = vm;
            }
        }

        #endregion


        #region Vote popup events

        /// <summary>
        /// The user has pressed vote from the popup
        /// </summary>
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
                // Get the next available vote and cast the vote
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

        /// <summary>
        /// The user has pressed cancel from the popup - just close it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void voteConfirmControl_CancelPressed(object sender, EventArgs e)
        {
            popupControl.IsOpen = false;
        }

        void playerStatsControl_CancelPressed(object sender, EventArgs e)
        {
            popupControl.IsOpen = false;
        }

        #endregion


        #region Api Events

        /// <summary>
        /// The api has returned
        /// </summary>
        void userApi_VoteCompleted(object sender, VoteEventArgs e)
        {
            // Turn off the progress bar
            SetProgressIndicator(false);

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // What player did they vote for
                PlayerRecord player = App.ViewModel.DbViewModel.Player(e.ServerResponse.Details.Vote.PlayerId);

                // Add the vote to the db
                App.ViewModel.DbViewModel.VoteSuccessfullyCast(e.ServerResponse.Details);

                // show a popup success message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = string.Format(AppResources.VoteSuccessful, player.LastName) }, new TimeSpan(0, 0, 2)));

                // Go back
                if (App.ViewModel.DbViewModel.OnPlayerListPage())
                {
                    if (!fromHome)
                    {
                        NavigationService.RemoveBackEntry();
                    }
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
        /// The api has returned - this one is just for the simulate vote routine
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

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            allowClicks = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("zone"))
            {
                string zone = NavigationContext.QueryString["zone"];
                zonePlayerIsIn = App.ViewModel.DbViewModel.Zone(Convert.ToInt32(zone));
            }

            if (NavigationContext.QueryString.ContainsKey("club"))
            {
                string club = NavigationContext.QueryString["club"];
                clubPlayerIsIn = App.ViewModel.DbViewModel.Club(Convert.ToInt32(club));
            }

            if (NavigationContext.QueryString.ContainsKey("country"))
            {
                string country = NavigationContext.QueryString["country"];
                countryPlayerIsIn = App.ViewModel.DbViewModel.Country(Convert.ToInt32(country));
            }

            if (NavigationContext.QueryString.ContainsKey("voteForPositionAt"))
            {
                int voteForPositionAt = Convert.ToInt32(NavigationContext.QueryString["voteForPositionAt"]);
                positionVotedFor = App.ViewModel.DbViewModel.Position(Convert.ToInt32(voteForPositionAt));
            }

            bool? isEurope = null;
            if (NavigationContext.QueryString.ContainsKey("isEurope"))
            {
                string eur = NavigationContext.QueryString["isEurope"];
                isEurope = eur.Contains("True") ? true : false;
            }
            else
            {
                isEurope = null;
            }

            fromHome = true;
            if (NavigationContext.QueryString.ContainsKey("fromHome"))
            {
                string h = NavigationContext.QueryString["fromHome"];
                fromHome = h.Contains("True") ? true : false;
            }

            // Do the default search
            PopulateList(zonePlayerIsIn, clubPlayerIsIn, countryPlayerIsIn, isEurope);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Turn off the popup here so we dont get flickering if turning off somewhere else and then going to the shop for example
            popupControl.IsOpen = false;
        }

        /// <summary>
        /// On back key press, remove the popup if it displayed
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (popupControl.IsOpen)
            {
                popupControl.IsOpen = false;
                e.Cancel = true;
            }
        }

        #endregion


        #region Helpers

        private void PopulateList(ZoneRecord zone, ClubRecord club, CountryRecord country, bool? isEurope)
        {
            // Alter the wording on the header
            PageHeaderControl.Setup(zone, club, country, isEurope);

            // Popuplate the lists
            if (club != null)
            {
                PlayersList.PlayersLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.PlayersList(club);
            }
            else if (zone != null)
            {
                PlayersList.PlayersLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.PlayersList(zone, isEurope);
            }
            else if (country != null)
            {
                PlayersList.PlayersLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.PlayersList(country);
            }
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