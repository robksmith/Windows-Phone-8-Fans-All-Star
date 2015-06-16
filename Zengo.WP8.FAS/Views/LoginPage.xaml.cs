
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
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class LoginPage : PhoneApplicationPage
    {
        #region Constructors

        public LoginPage()
        {
            InitializeComponent();

            LoginControl.LoginStarting += LoginControl_LoginStarting;
            LoginControl.LoginCompleted += LoginControl_LoginCompleted;
            LoginControl.RemoveKeyboard += LoginControl_RemoveKeyboard;

            PageHeaderControl.PageTitle = AppResources.ProductTitle;
            PageHeaderControl.PageName = AppResources.LoginPageTitle;

            BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarLogin };
            button.Click += AppBarMenuLogin_Click;
            ApplicationBar.Buttons.Add(button);
        }

        #endregion


        #region Page Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If required, remove an entry from the back stack - this is used if we get here from the settings page
            if (NavigationContext.QueryString.ContainsKey("removeBackStack"))
            {
                string removeBackStack = NavigationContext.QueryString["removeBackStack"];
                if (removeBackStack == "yes")
                {
                    NavigationService.RemoveBackEntry();
                    // we need to remove this in case they press back from another page after this - although at present the login page doesnt go anywhere else
                    NavigationContext.QueryString.Remove("removeBackStack");
                }
            }
        }

        #endregion


        #region Login Control Events

        /// <summary>
        /// They have pressed the login tick button
        /// </summary>
        void LoginControl_LoginStarting(object sender, Controls.LoginStartingEventArgs e)
        {
            ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            b.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // turn on the progress bar
            SetProgressIndicator(true);
            //LoadingHelper.Instance.IsLoading = true;
            //PageHeaderControl.EnableProgresBar(true);
        }

        /// <summary>
        /// The login process has finished
        /// </summary>
        void LoginControl_LoginCompleted(object sender, Controls.LoginCompletedEventArgs e)
        {
            // turn off the progress bar
            SetProgressIndicator(false);

            if (e.Success)
            {
                // show a success message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = e.Message }, new TimeSpan(0, 0, 3)));

                // Remove items from the back stack and go home
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                // Show a message
                MessageBox.Show(e.Message, "Login Failed", MessageBoxButton.OK);

                // re-enable the app bar
                ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                b.IsEnabled = true;
            }
        }

        void LoginControl_RemoveKeyboard(object sender, EventArgs e)
        {
            this.Focus();
        }

        #endregion


        #region App Bar Events

        private void AppBarMenuLogin_Click(object sender, EventArgs e)
        {
            LoginControl.Login();
        }

        #endregion


        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = "Logging in"
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

    }
}