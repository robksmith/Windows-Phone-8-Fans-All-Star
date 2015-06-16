
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WepApi.Api;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Fields

        // The two popups posible from this page
        Popup managerPopupControl;
        ManagerEuropeControl europeManagerControl;
        ManagerRotwControl rotwManagerControl;

        ApplicationBarMenuItem menuItemToggleVoteCount;
        ApplicationBarMenuItem menuItemUpdates;
        ApplicationBarMenuItem menuItemToggleMyTeam;

        #endregion


        #region Constructors

        public MainPage()
        {
            InitializeComponent();

            // Subscribe to register/login control events
            RegisterOrLoginControl.RegisterPressed += RegisterOrLoginControl_RegisterPressed;
            RegisterOrLoginControl.LoginPressed += RegisterOrLoginControl_LoginPressed;
            RegisterOrLoginControl.LaterPressed += RegisterOrLoginControl_LaterPressed;
            RegisterOrLoginControl.ResetPasswordPressed += RegisterOrLoginControl_ResetPasswordPressed;
            //RegisterOrLoginControl.IHavePinPressed += RegisterOrLoginControl_IHavePinPressed;

            SubmitButton.ButtonTapped += SubmitButton_Tapped;
            // Subscribe to the player tapped event
            PitchEurope.PlayerTapped += Pitch_PlayerTapped;
            //PitchEurope.ManagerTapped += PitchEurope_ManagerTapped;

            PitchRotw.PlayerTapped += Pitch_PlayerTapped;
            //PitchRotw.ManagerTapped += PitchRotw_ManagerTapped;

            PitchFull.PlayerTapped += Pitch_PlayerTapped;

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            // Create the popups
            managerPopupControl = new Popup();

            // Create the vote popup for managers
            europeManagerControl = new ManagerEuropeControl();
            //europeManagerControl.ClosePressed += managerControl_ClosePressed;

            // Create the vote popup for managers
            rotwManagerControl = new ManagerRotwControl();
            //rotwManagerControl.ClosePressed += managerControl_ClosePressed;

            managerPopupControl.VerticalOffset = 0;

            //BuildApplicationBar();

            // Push notification stuff

            string channelName = "FASChannel";

            // Try to find push channel

            HttpNotificationChannel pushChannel = HttpNotificationChannel.Find(channelName);

            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += PushChannelOnChannelUriUpdated;
                pushChannel.ErrorOccurred += PushChannelOnErrorOccurred;

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += PushChannelOnShellToastNotificationReceived;

                pushChannel.Open();

                // Bind this channel for toast events
                pushChannel.BindToShellToast();
                
            }
            else
            {
                // The channel was already open, just register to the events
                pushChannel.ChannelUriUpdated += PushChannelOnChannelUriUpdated;
                pushChannel.ErrorOccurred += PushChannelOnErrorOccurred;

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += PushChannelOnShellToastNotificationReceived;

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());
                PushshenApi api = new PushshenApi { DeviceUri = pushChannel.ChannelUri.ToString() };
                api.Start();
            }
        }

        #endregion


        #region Push Event handlers

        private void PushChannelOnShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                }
            }

            // Display a dialog of all the fields in the toast.
            Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));
        }

        private void PushChannelOnErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }

        private void PushChannelOnChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());

                PushshenApi api = new PushshenApi { DeviceUri = e.ChannelUri.ToString() };
                api.Start();

            });
        }

        #endregion


        #region Page Event Handlers

        /// <summary>
        /// This is where we try and do an update
        /// </summary>
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            GridGrayedOut.Visibility = System.Windows.Visibility.Collapsed;

            // Remove all of the back entries so that back from the main page quits the app
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            // Set off the update timer
            if (DatabaseHelper.FireUpdates)
            {
                App.JsonDataPollHelper.Start();
            }

            // Kick off the timer which polls for new transactions - because transactions arent loaded 
            App.UserDataPollHelper.Start(9990, 9996);

            // Draw the text in the icons
            ReloadPitchIcons();
        }

        /// <summary>
        /// This also gets run when we navigate back from the clubs list
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Create the app bar
            BuildApplicationBar();

            // Turn off teh login popup
            PopupRegisterLogin(false);

            // Set the page names
            EuropeROTWHeaderControl.PageName = AppResources.ROTWName;
            EuropePivotHeaderControl.PageName = AppResources.EuropeName;
            EuropeFullHeaderControl.PageName = AppResources.FullViewPivot;

            // Dont show submit team button if they are not logged in
            if (App.ViewModel.DbViewModel.IsLoggedOn())
            {
                SubmitButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                SubmitButton.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #endregion


        #region App Bar Events

        void menuItemJsonHttps_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/BuyVotesAfterRegistrationPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Go to the settings page
        /// </summary>
        void AppBarMenuItemSettings_Click(object sender, EventArgs e)
        {
            if (App.ViewModel.DbViewModel.IsLoggedOn() == false)
            {
                PopupRegisterLogin(true);
            }
            else
            {
                PopupRegisterLogin(false);

                // copy in the temporary teams
                App.AppConstants.FirstFavTeam = App.ViewModel.DbViewModel.FirstFavouriteClub();
                App.AppConstants.SecondFavTeam = App.ViewModel.DbViewModel.SecondFavouriteClub();
                App.AppConstants.MyCountry = App.ViewModel.DbViewModel.MyCountry();

                this.NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        /// <summary>
        /// Go to players search page
        /// </summary>
        void AppBarMenuItemPlayers_Click(object sender, EventArgs e)
        {
            if (App.AppConstants.RequireLogonToNavigateFromHomePage && App.ViewModel.DbViewModel.IsLoggedOn() == false)
            {
                PopupRegisterLogin(true);
            }
            else
            {
                PopupRegisterLogin(false);
                this.NavigationService.Navigate(new Uri("/Views/PlayerFindPage.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Go to the shop
        /// </summary>
        void AppBarMenuItemShop_Click(object sender, EventArgs e)
        {
            if (App.AppConstants.RequireLogonToNavigateFromHomePage && App.ViewModel.DbViewModel.IsLoggedOn() == false)
            {
                PopupRegisterLogin(true);
            }
            else
            {
                PopupRegisterLogin(false);
                this.NavigationService.Navigate(new Uri("/Views/BuyVotesPage.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Go to Languages page
        /// </summary>
        private void AppBarMenuItemLanguage_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/LanguagePage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Go to the help page
        /// </summary>
        void AppBarMenuItemHelp_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/HelpPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Go to History Page
        /// </summary>
        private void AppBarMenuItemHistory_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/HistoryPage.xaml", UriKind.Relative));
        }


        /// <summary>
        /// Go to the updates page
        /// </summary>
        void menuItemUpdates_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/UpdateCheckPage.xaml", UriKind.Relative));
        }

        void menuItemToggleVoteCount_Click(object sender, EventArgs e)
        {
            App.ViewModel.ShowVotesPerPosition = App.ViewModel.ShowVotesPerPosition == true ? false : true;

            // Change the text in the menu 
            SetupVoteText();

            // Re-draw the icons to add/remove the vote count
            ReloadPitchIcons();
        }

        void menuItemToggleMyTeam_Click(object sender, EventArgs e)
        {
            App.ViewModel.ShowMyTeam = App.ViewModel.ShowMyTeam == true ? false : true;

            // Change the text in the menu 
            SetupMyTeamText();

            // Re-draw the icons to add/remove the vote count
            ReloadPitchIcons();
        }

        #endregion


        #region Events for the register/login popup control

        void RegisterOrLoginControl_RegisterPressed(object sender, EventArgs e)
        {
            //PopupRegisterLogin(false);

            // copy in the temporary teams
            App.AppConstants.FirstFavTeam = null;
            App.AppConstants.SecondFavTeam = null;
            App.AppConstants.MyCountry = null;

            this.NavigationService.Navigate(new Uri("/Views/RegisterPage.xaml", UriKind.Relative));
        }

        void RegisterOrLoginControl_LoginPressed(object sender, EventArgs e)
        {
            //PopupRegisterLogin(false);
            this.NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.Relative));
        }

        void RegisterOrLoginControl_LaterPressed(object sender, EventArgs e)
        {
            PopupRegisterLogin(false);
        }

        void RegisterOrLoginControl_ResetPasswordPressed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/PasswordResetPage.xaml", UriKind.Relative));
        }

        #endregion


        #region Submit and Player Button Event Handlers

        void SubmitButton_Tapped(object sender, SubmitIconTapEventArgs e)
        {
            // Check they are logged in
            if (App.ViewModel.DbViewModel.IsLoggedOn() == false)
            {
                MessageBox.Show("Please login first before you submit a team");
                return;
            }

            // Go to the submit team page
            this.NavigationService.Navigate(new Uri("/Views/SubmitTeamPage.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// A player icon has been tapped
        /// </summary>
        void Pitch_PlayerTapped(object sender, PlayerIconTapEventArgs e)
        {
            if (App.AppConstants.RequireLogonToNavigateFromHomePage && App.ViewModel.DbViewModel.IsLoggedOn() == false)
            {
                PopupRegisterLogin(true);
            }
            else
            {
                PopupRegisterLogin(false);

                string eur = e.VoteForPositionAt.IsEurope == true ? "True" : "False";

                GridGrayedOut.Visibility = System.Windows.Visibility.Visible;

                string url = string.Format("/Views/PlayerListPage.xaml?zone={0}&isEurope={1}&voteForPositionAt={2}&fromHome={3}", e.Zone.ZoneId, eur, e.VoteForPositionAt.PositionId, "True");

                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        //void PitchRotw_ManagerTapped(object sender, ManagerIconTapEventArgs e)
        //{
        //    // Close the popupout
        //    if (PitchRotw.IsManagerOpen())
        //    {
        //        PitchRotw.CloseManager();
        //    }

        //    // Disable the app bar
        //    EnableAppBar(false);

        //    // open the popup
        //    managerPopupControl.Child = rotwManagerControl;
        //    managerPopupControl.IsOpen = true;
        //}

        //void PitchEurope_ManagerTapped(object sender, ManagerIconTapEventArgs e)
        //{
        //    // Close the popupout
        //    if (PitchEurope.IsManagerOpen())
        //    {
        //        PitchEurope.CloseManager();
        //    }

        //    // Disable the app bar
        //    EnableAppBar(false);

        //    // open the popup
        //    managerPopupControl.Child = europeManagerControl;
        //    managerPopupControl.IsOpen = true;
        //}


        //void managerControl_ClosePressed(object sender, EventArgs e)
        //{
        //    // Close the control
        //    managerPopupControl.IsOpen = false;

        //    // Re-enable the app bar
        //    EnableAppBar(true);
        //}

        #endregion


        #region Back Key

        /// <summary>
        /// If we have the register dialog up, then the back key acts as a cancellation of this
        /// </summary>
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (managerPopupControl.IsOpen)
            {
                // Close the popup
                managerPopupControl.IsOpen = false;
                
                // Re-enable the app bar
                EnableAppBar(true);

                e.Cancel = true;
            }

            if (RegisterOrLoginControl.IsVisible())
            {
                PopupRegisterLogin(false);
                e.Cancel = true;
            }

            if (PitchEurope.IsSubstitutesOpen())
            {
                PitchEurope.CloseSubstitutes();
                e.Cancel = true;
            }

            //if (PitchEurope.IsManagerOpen())
            //{
            //    PitchEurope.CloseManager();
            //    e.Cancel = true;
            //}

            if (PitchRotw.IsSubstitutesOpen())
            {
                PitchRotw.CloseSubstitutes();
                e.Cancel = true;
            }

            //if (PitchRotw.IsManagerOpen())
            //{
            //    PitchRotw.CloseManager();
            //    e.Cancel = true;
            //}
        }

        #endregion


        #region Helpers

        public void ReloadPitchIcons()
        {
            PitchEurope.Reload(null);
            PitchRotw.Reload(null);
            PitchFull.Reload(null);
        }

        /// <summary>
        /// Create the app bar
        /// </summary>
        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/settings.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarSettings };
            button.Click += AppBarMenuItemSettings_Click;
            ApplicationBar.Buttons.Add(button);

            button = new ApplicationBarIconButton(new Uri("/Images/AppBar/search.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarSearch };
            button.Click += AppBarMenuItemPlayers_Click;
            ApplicationBar.Buttons.Add(button);

            button = new ApplicationBarIconButton(new Uri("/Images/AppBar/shop.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarShop };
            button.Click += AppBarMenuItemShop_Click;
            ApplicationBar.Buttons.Add(button);

            button = new ApplicationBarIconButton(new Uri("/Images/AppBar/help.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarHelp };
            button.Click += AppBarMenuItemHelp_Click;
            ApplicationBar.Buttons.Add(button);

            // Clear the menu items
            ApplicationBar.MenuItems.Clear();

            // Menu - show updates
            if (App.AppConstants.EnableUpdatesMenu)
            {
                menuItemUpdates = new ApplicationBarMenuItem();
                menuItemUpdates.Text = "Develop:show updates";
                ApplicationBar.MenuItems.Add(menuItemUpdates);
                menuItemUpdates.Click += menuItemUpdates_Click;
            }
#if DEBUG
            // Language menu
            //var item = new ApplicationBarMenuItem(AppResources.AppBarLanguage);
            //item.Click += AppBarMenuItemLanguage_Click;
            //ApplicationBar.MenuItems.Add(item);

            if (App.ViewModel.DbViewModel.IsLoggedOn())
            {
                menuItemToggleMyTeam = new ApplicationBarMenuItem();
                SetupMyTeamText();
                ApplicationBar.MenuItems.Add(menuItemToggleMyTeam);
                menuItemToggleMyTeam.Click += menuItemToggleMyTeam_Click; ;

                menuItemToggleVoteCount = new ApplicationBarMenuItem();
                SetupVoteText();
                ApplicationBar.MenuItems.Add(menuItemToggleVoteCount);
                menuItemToggleVoteCount.Click += menuItemToggleVoteCount_Click;

                ApplicationBarMenuItem buyVotesAfterRegistration = new ApplicationBarMenuItem();
                buyVotesAfterRegistration.Text = "Develop:buy votes after register";
                ApplicationBar.MenuItems.Add(buyVotesAfterRegistration);
                buyVotesAfterRegistration.Click += menuItemJsonHttps_Click;
            }
#endif
        }

        private void PopupRegisterLogin(bool enable)
        {
            if (enable)
            {
                // Enable the popup
                RegisterOrLoginControl.Enable();

                // Disable the pivot
                MainPivot.IsEnabled = false;

                // Disable the app bar
                EnableAppBar(false);
            }
            else
            {
                // Turn off the popup
                RegisterOrLoginControl.Disable();

                // Enable the pivot
                MainPivot.IsEnabled = true;

                // Enable the app bar
                EnableAppBar(true);
            }
        }

        private void EnableAppBar(bool enable)
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = enable;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = enable;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = enable;
            (ApplicationBar.Buttons[3] as ApplicationBarIconButton).IsEnabled = enable;
        }

        /// <summary>
        /// Toggle show vote text
        /// </summary>
        private void SetupVoteText()
        {
            if (App.ViewModel.ShowVotesPerPosition)
            {
                menuItemToggleVoteCount.Text = "develop:hide vote count per position";
            }
            else
            {
                menuItemToggleVoteCount.Text = "develop:show vote count per position";
            }
        }

        private void SetupMyTeamText()
        {
            if (App.ViewModel.ShowMyTeam)
            {
                menuItemToggleMyTeam.Text = "develop:show my votes in cast order";
            }
            else
            {
                menuItemToggleMyTeam.Text = "develop:show my team (1 player per pos)";
            }
        }

        #endregion
    }
}