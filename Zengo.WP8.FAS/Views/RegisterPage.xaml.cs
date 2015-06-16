
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class RegisterPage : PhoneApplicationPage
    {
        #region Fields

        ApplicationBarIconButton registerButton;

        #endregion


        #region Constructors

        public RegisterPage()
        {
            InitializeComponent();

            // Register for the control events that tell us the team selector button has been pressed
            RegisterControl.FirstFavouriteTeamPressed += RegisterControl_FirstFavouriteTeamPressed;
            RegisterControl.SecondFavouriteTeamPressed += RegisterControl_SecondFavouriteTeamPressed;
            RegisterControl.MyCountryPressed += RegisterControl_MyCountryPressed;

            RegisterControl.RegisterStarting += RegisterControl_RegisterStarting;
            RegisterControl.RegisterCompleted += RegisterControl_RegisterCompleted;

            PageHeaderControl.PageName = AppResources.RegistrationPage;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;

            RegisterControl.Page = this;

            BuildApplicationBar();

            // Get our register button so we can enable/disable it
            registerButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarRegister };
            button.Click += AppBarMenuRegister_Click;
            ApplicationBar.Buttons.Add(button);

        }

        #endregion


        #region Page Event Handlers

        /// <summary>
        /// This also gets run when we navigate back from the clubs list and countries list
        /// </summary>
        protected override void OnNavigatedTo( NavigationEventArgs e) 
        {
            // Fill in the hidden textbox in case they have changed favourite team
            RegisterControl.SetTeamsAndCountry(App.AppConstants.FirstFavTeam, App.AppConstants.SecondFavTeam, App.AppConstants.MyCountry);

            RegisterControl.ResetErrors();
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// They have pressed the button to select a first favourite team
        /// </summary>
        void RegisterControl_FirstFavouriteTeamPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteClubPage.xaml?Team=1", UriKind.Relative));
        }

        /// <summary>
        /// They have pressed the button to select a second favourite team
        /// </summary>
        void RegisterControl_SecondFavouriteTeamPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FavouriteClubPage.xaml?Team=2", UriKind.Relative));
        }

        void RegisterControl_MyCountryPressed(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/MyCountryPage.xaml", UriKind.Relative));
        }


        void RegisterControl_RegisterStarting(object sender, EventArgs e)
        {
            // Disable the button
            registerButton.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // turn on the progress bar
            SetProgressIndicator(true);
        }

        void RegisterControl_RegisterCompleted(object sender, RegisterControl.RegisterCompletedEventArgs e)
        {
            // turn off the progress bar
            SetProgressIndicator(false);

            // turn back on the button
            registerButton.IsEnabled = true;

            if (e.Success)
            {
                // show a success message
                MessageBox.Show("Registration complete! Don't forget to click on the link emailed to you to activate your account.", "Registration Success", MessageBoxButton.OK);
                
                //The new flow wants us to go to the shop page with instructions to remove the settings page from the back stack
                this.NavigationService.Navigate(new Uri("/Views/BuyVotesAfterRegistrationPage.xaml?removeBackStack=yes", UriKind.Relative));
            }
            else
            {
                // Show a message
                MessageBox.Show(e.Message, "Registration Failed", MessageBoxButton.OK);
            }
        }

        #endregion


        #region App Bar Events

        private void AppBarMenuRegister_Click(object sender, EventArgs e)
        {
            RegisterControl.Register(RegisterScrollViewer);
        }

        #endregion


        #region Helpers

        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = "Registering"
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion

    }
}