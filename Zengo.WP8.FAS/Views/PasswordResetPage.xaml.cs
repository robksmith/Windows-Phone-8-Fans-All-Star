
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using System;
using System.Windows;
using System.Windows.Navigation;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class PasswordResetPage : PhoneApplicationPage
    {
        #region Fields

        ApplicationBarIconButton requestPinAppBarButton;

        #endregion


        #region Constructors

        public PasswordResetPage()
        {
            InitializeComponent();

            RequestPinControl.PasswordResetStarting += RequestPinControl_RequestPinStarting;
            RequestPinControl.PasswordResetCompleted += RequestPinControl_RequestPinCompleted;
            RequestPinControl.RemoveKeyboard += LoginControl_RemoveKeyboard;

            requestPinAppBarButton = (ApplicationBarIconButton)ApplicationBar.Buttons[0];

            PageHeaderControl.PageName = AppResources.ResetPasswordPage;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;

            BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarResetPassword };
            button.Click += AppBarMenuSendPin_Click;
            ApplicationBar.Buttons.Add(button);

        }

        #endregion


        #region Sub Control Events

        /// <summary>
        /// They have pressed the request pin tick button
        /// </summary>
        void RequestPinControl_RequestPinStarting(object sender, Controls.PasswordResetStartingEventArgs e)
        {
            // Disable the button
            requestPinAppBarButton.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // turn on the progress bar
            SetProgressIndicator(true);
        }

        /// <summary>
        /// The request pin api process has finished
        /// </summary>
        void RequestPinControl_RequestPinCompleted(object sender, Controls.PasswordResetCompletedEventArgs e)
        {
            // turn off the progress bar
            SetProgressIndicator(false);

            // Turn on the button
            requestPinAppBarButton.IsEnabled = true;

            if (e.Success)
            {
                // show a success message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = e.Message }, new TimeSpan(0, 0, 3)));

                // Remove items from the back stack and go home
                NavigationService.GoBack();
            }
            else
            {
                // Show a message
                MessageBox.Show(e.Message, "Reset Password Failed", MessageBoxButton.OK);
            }
        }

        void LoginControl_RemoveKeyboard(object sender, EventArgs e)
        {
            this.Focus();
        }

        #endregion


        #region App Bar Events

        private void AppBarMenuSendPin_Click(object sender, EventArgs e)
        {
            RequestPinControl.RequestPin();
        }

        #endregion


        #region Helpers

        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = "Resetting Password"
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion
    }
}