
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
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class PasswordUpdatePage : PhoneApplicationPage
    {
        #region Fields

        ApplicationBarIconButton changePasswordAppBarButton;

        #endregion


        #region Constructors

        public PasswordUpdatePage()
        {
            InitializeComponent();

            EnterPinControl.PasswordUpdateStarting += EnterPinControl_EnterPinStarting;
            EnterPinControl.PasswordUpdateCompleted += EnterPinControl_EnterPinCompleted;
            EnterPinControl.RemoveKeyboard += LoginControl_RemoveKeyboard;

            BuildApplicationBar();
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            changePasswordAppBarButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/check.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarUpdatePassword };
            changePasswordAppBarButton.Click += AppBarMenuEnterPin_Click;
            ApplicationBar.Buttons.Add(changePasswordAppBarButton);

        }

        #endregion


        #region Login Control Events


        /// <summary>
        /// They have pressed the login tick button
        /// </summary>
        void EnterPinControl_EnterPinStarting(object sender, Controls.PasswordUpdateControl.PasswordUpdateStartingEventArgs e)
        {
            // Disable the button
            changePasswordAppBarButton.IsEnabled = false;

            // Get rid of the keyboard
            this.Focus();

            // turn on the progress bar
            SetProgressIndicator(true);
        }

        /// <summary>
        /// The login process has finished
        /// </summary>
        void EnterPinControl_EnterPinCompleted(object sender, Controls.PasswordUpdateControl.PasswordUpdateCompletedEventArgs e)
        {
            // turn off the progress bar
            SetProgressIndicator(false);

            // Turn on the button
            changePasswordAppBarButton.IsEnabled = true;

            if (e.Success)
            {
                // show a success message
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = e.Message }, new TimeSpan(0, 0, 3)));

                // Remove items from the back stack and go home
                NavigationService.GoBack();
            }
            else
            {
                // Show the error message
                MessageBox.Show(e.Message, AppResources.PasswordResetFailed, MessageBoxButton.OK);
            }
        }

        void LoginControl_RemoveKeyboard(object sender, EventArgs e)
        {
            this.Focus();
        }

        #endregion


        #region App Bar Events

        private void AppBarMenuEnterPin_Click(object sender, EventArgs e)
        {
            EnterPinControl.EnterPin();
        }

        #endregion


        #region Helpers

        void SetProgressIndicator(bool enabled)
        {
            ProgressIndicator progress = new ProgressIndicator
            {
                IsVisible = enabled,
                IsIndeterminate = true,
                Text = AppResources.ChangingPassword
            };

            SystemTray.SetProgressIndicator(this, progress);
        }

        #endregion
    }
}