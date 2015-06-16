
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Windows;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        #region Fields

        UserApi userApi;
        ApplicationBarIconButton saveChanges;
        #endregion


        #region Constructors

        public SettingsPage()
        {
            InitializeComponent();

            // Register for the pivot selection changed event
            PivotControl.SelectionChanged += PivotControl_SelectionChanged;

            // Register for sub control events
            MyAccountControl.UpdateAccountStarting += MyAccountControl_UpdateAccountStarting;
            MyAccountControl.UpdateAccountCompleted += MyAccountControl_UpdateAccountCompleted;
            //MyAccountControl.RemoveKeyboard += MyAccountControl_RemoveKeyboard;

            // Register for the control events that tell us the team selector button has been pressed
            MyAccountControl.FirstFavouriteTeamPressed += RegisterControl_FirstFavouriteTeamPressed;
            MyAccountControl.SecondFavouriteTeamPressed += RegisterControl_SecondFavouriteTeamPressed;
            MyAccountControl.MyCountryPressed += MyAccountControl_MyCountryPressed;

            TeamHistory.pitchHistoryLongList.SelectionChanged += pitchHistoryLongList_SelectionChanged;

            // Create an app bar
            ApplicationBar = new ApplicationBar();

            userApi = new UserApi();
            userApi.ResendActivationEmailCompleted += userApi_ResendValidationEmailCompleted;

            // Populate our lists
            PopulateList();

            PurchasesPage.PageName = AppResources.PurchasesTitle;
            AccountPage.PageName = AppResources.AccountsTitle;
            VotesPage.PageName = AppResources.VotesTitle;
            TeamPage.PageName = "my teams";
        }

        #endregion

        void pitchHistoryLongList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PitchRecord pitch = TeamHistory.pitchHistoryLongList.SelectedItem as PitchRecord;

            if (pitch != null)
            {
                this.NavigationService.Navigate(new Uri("/Views/PitchPage.xaml?Pitch=" + pitch.PitchId, UriKind.Relative));
            }
        }

        #region Page Event Handlers

        /// <summary>
        /// This also gets run when we navigate back from the clubs list and from the activate page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TeamHistory.pitchHistoryLongList.SelectedItem = null;


            // Set the data context for the Profile view.
            MyAccountControl.DataContext = App.ViewModel.DbViewModel.CurrentUser;
            DataContext = App.ViewModel.DbViewModel;

            // From the hidden team and country convert them to team names
            MyAccountControl.SetFavouriteTeamFromHiddenId(App.AppConstants.FirstFavTeam, App.AppConstants.SecondFavTeam, App.AppConstants.MyCountry);

            // Set up the default app bar
            CreateApplicationBarMyAccount();

            // Enable disable the activate button
            if (!App.ViewModel.DbViewModel.CurrentUser.IsValidated)
            {
                HyperlinkValidate.Visibility = System.Windows.Visibility.Visible;
                HyperlinkResendValidationEmail.Visibility = System.Windows.Visibility.Visible;
                TextblockNid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                HyperlinkValidate.Visibility = System.Windows.Visibility.Collapsed;
                HyperlinkResendValidationEmail.Visibility = System.Windows.Visibility.Collapsed;
                TextblockNid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        #endregion


        #region Event Handlers

        private void HyperlinkResendValidationEmail_Click(object sender, RoutedEventArgs e)
        {
            // Turn on the progress bar
            SetProgressIndicator(AppResources.ResendEmailMessage, true);

            // Turn off the button to stop multiple presses
            HyperlinkResendValidationEmail.IsEnabled = false;

            // Disable the app bar button - its already disabled if the acount isn't activated
            //saveChanges.IsEnabled = false;

            // Do the api call
            userApi.ResendActivationEmail(App.ViewModel.DbViewModel.CurrentUser.Email, App.ViewModel.DbViewModel.CurrentUser.UserId);
        }

        /// <summary>
        /// They have pressed the button to select a first favourite team on the sub control
        /// </summary>
        void RegisterControl_FirstFavouriteTeamPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteClubPage.xaml?Team=1", UriKind.Relative));
        }

        /// <summary>
        /// They have pressed the button to select a second favourite team on the sub control
        /// </summary>
        void RegisterControl_SecondFavouriteTeamPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteClubPage.xaml?Team=2", UriKind.Relative));
        }

        /// <summary>
        /// User wants to alter country
        /// </summary>
        void MyAccountControl_MyCountryPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/MyCountryPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// When the pivot selection changes we want to alter the app bar
        /// </summary>
        void PivotControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // close the keyboard if its open
            this.Focus();

            // enable disable app bar depending on the pivot
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    CreateApplicationBarMyAccount();
                    break;

                case 1:
                    CreateApplicationBarForHistory();
                    break;

                case 2:
                    CreateApplicationBarForHistory();
                    break;
            }
        }

        /// <summary>
        /// The sub control tells us that a save is about to start
        /// </summary>
        void MyAccountControl_UpdateAccountStarting(object sender, Controls.UpdateAccountStartingEventArgs e)
        {
            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            b.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // turn on the progress bar
            SetProgressIndicator(true);
        }


        /// <summary>
        /// The sub control tells us that a save has finished
        /// </summary>
        void MyAccountControl_UpdateAccountCompleted(object sender, Controls.UpdateAccountCompletedEventArgs e)
        {
            SetProgressIndicator(false);

            // If update successful, go to another page
            if (e.Success)
            {
                // show a success message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = e.Message }, new TimeSpan(0, 0, 3)));

                // Send back a page - which should now be to the home page
                NavigationService.GoBack();
            }
            else
            {
                // Show the error message
                MessageBox.Show(e.Message, AppResources.UpdateAccountFailed, MessageBoxButton.OK);

                ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                b.IsEnabled = true;
            }

        }

        void MyAccountControl_RemoveKeyboard(object sender, EventArgs e)
        {
            this.Focus();
        }

        #endregion


        #region App Bar Events

        /// <summary>
        /// Save changes
        /// </summary>
        void AppBarMenuItemUpdate_Click(object sender, EventArgs e)
        {
            // Save is called from the account control 
            MyAccountControl.Save(RegisterScrollViewer);
        }

        /// <summary>
        /// logout
        /// </summary>
        void AppBarMenuItemLogout_Click(object sender, EventArgs e)
        {
            // Log them out
            App.ViewModel.DbViewModel.Logout();

            // show a success message
            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = AppResources.LoggedOutMessage}, new TimeSpan(0, 0, 3)));

            // Send them to the main page
            //if (NavigationService.CanGoBack)
            //{
            //    NavigationService.GoBack();
            //}

            // Now on logout we send them to the login page with instructions to remove the settings page from the back stack
            this.NavigationService.Navigate(new Uri("/Views/LoginPage.xaml?removeBackStack=yes", UriKind.Relative));
        }

        /// <summary>
        /// Change password
        /// </summary>
        void AppBarMenuItemChangePassword_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/PasswordUpdatePage.xaml", UriKind.Relative));
        }


        ///// <summary>
        ///// Request pin
        ///// </summary>
        //void AppBarMenuItemRequestPasswordResetPin_Click(object sender, EventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/PasswordResetPinRequestPage.xaml", UriKind.Relative));
        //}


        ///// <summary>
        ///// I have pin
        ///// </summary>
        //private void AppBarMenuItemEnterPasswordResetPin_Click(object sender, System.EventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/PasswordResetPinEnterPage.xaml", UriKind.Relative));
        //}

        #endregion


        #region Helpers

        /// <summary>
        /// Populate our vote history and purchase history
        /// </summary>
        private void PopulateList()
        {
            VotingHistory.playersLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.VotingHistory();

            TransactionHistory.transactionLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.TransactionHistory();

            TeamHistory.pitchHistoryLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.PitchHistory();
        }


        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = AppResources.UpdatingAccount
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion


        #region App Bar Creation

        /// <summary>
        /// Create our application bar for my account pivot
        /// </summary>
        void CreateApplicationBarMyAccount()
        {
            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // The save button
            saveChanges = new ApplicationBarIconButton();
            saveChanges.IconUri = new Uri("/Images/AppBar/save.png", UriKind.Relative);
            saveChanges.Text = AppResources.SaveChanges;
            saveChanges.Click += new EventHandler(AppBarMenuItemUpdate_Click);
            if (!App.ViewModel.DbViewModel.CurrentUser.IsValidated)
            {
                saveChanges.IsEnabled = false;
            }
            ApplicationBar.Buttons.Add(saveChanges);

            CreateStandardMenuItems();
        }

        /// <summary>
        /// Create app bar for my votes and my purchases
        /// </summary>
        void CreateApplicationBarForHistory()
        {
            //ApplicationBar = new ApplicationBar();

            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            // The save button
            ApplicationBarIconButton saveChanges = new ApplicationBarIconButton();
            saveChanges.IsEnabled = false;
            saveChanges.IconUri = new Uri("/Images/AppBar/save.png", UriKind.Relative);
            saveChanges.Text = AppResources.SaveChanges;
            saveChanges.Click += new EventHandler(AppBarMenuItemUpdate_Click);
            ApplicationBar.Buttons.Add(saveChanges);

            CreateStandardMenuItems();
        }

        private void CreateStandardMenuItems()
        {
            ApplicationBarMenuItem changePassword = new ApplicationBarMenuItem();
            changePassword.Text = AppResources.ChangeMyPassword;
            ApplicationBar.MenuItems.Add(changePassword);
            changePassword.Click += new EventHandler(AppBarMenuItemChangePassword_Click);

            // Only show the "activate account" menu bar item when the user isn't activated
            //if (!App.ViewModel.DbViewModel.CurrentUser.IsValidated)
            //{
            //    ApplicationBarMenuItem activateAccount = new ApplicationBarMenuItem();
            //    activateAccount.Text = "activate account";
            //    ApplicationBar.MenuItems.Add(activateAccount);
            //    activateAccount.Click += new EventHandler(AppBarMenuItemActivateAccount_Click);
            //}

            //ApplicationBarMenuItem sendMePin = new ApplicationBarMenuItem();
            //sendMePin.Text = "send me a password reset pin";
            //ApplicationBar.MenuItems.Add(sendMePin);
            //sendMePin.Click += new EventHandler(AppBarMenuItemRequestPasswordResetPin_Click);

            //ApplicationBarMenuItem iHavePin = new ApplicationBarMenuItem();
            //iHavePin.Text = "i have a password reset pin";
            //ApplicationBar.MenuItems.Add(iHavePin);
            //iHavePin.Click += new EventHandler(AppBarMenuItemEnterPasswordResetPin_Click);

            ApplicationBarMenuItem logout = new ApplicationBarMenuItem();
            logout.Text = "logout";
            ApplicationBar.MenuItems.Add(logout);
            logout.Click += AppBarMenuItemLogout_Click;
        }

        #endregion


        #region Resend Email Events

        void userApi_ResendValidationEmailCompleted(object sender, ResendPinEventArgs e)
        {
            // Turn off the progress bar
            SetProgressIndicator("", false);

            // Turn back on the button and the app bar menu
            HyperlinkResendValidationEmail.IsEnabled = true;

            // Disable the app bar button - its already disabled if the acount isn't activated - no need to re-enable it
            //saveChanges.IsEnabled = true;

            // Show a message
            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                // On successful re send, we disable the button to deter rapid presses
                //TextBlockResendDescription.Visibility = System.Windows.Visibility.Collapsed;
                HyperlinkResendValidationEmail.Visibility = System.Windows.Visibility.Collapsed;

                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = e.ServerResponse.Error.Message }, new TimeSpan(0, 0, 3)));
            }
            else
            {
                MessageBox.Show(e.FriendlyMessage, AppResources.ResendEmailFailed, MessageBoxButton.OK);
            }
        }

        #endregion


        void SetProgressIndicator(string text, bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = text
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

    }
}