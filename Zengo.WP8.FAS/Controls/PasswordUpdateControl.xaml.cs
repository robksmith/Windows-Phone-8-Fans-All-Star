
#region Usings

using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PasswordUpdateControl : UserControl
    {
        public class PasswordUpdateStartingEventArgs : EventArgs
        {
            public int Count { get; set; }
        }

        public class PasswordUpdateCompletedEventArgs : EventArgs
        {
            public bool Success { get; set; }

            public string Message { get; set; }
        }

        #region Events

        public event EventHandler<PasswordUpdateStartingEventArgs> PasswordUpdateStarting;
        public event EventHandler<PasswordUpdateCompletedEventArgs> PasswordUpdateCompleted;
        public event EventHandler<EventArgs> RemoveKeyboard;

        #endregion


        #region Fields

        UserApi userApi;

        //const string emailWatermarkText = "enter your registered email address";
        //const string pinWatermarkText = "enter the pin we emailed to you";
         string passwordCurrentWatermark = AppResources.ExistingPasswordWatermark;
         string passwordWatermark = AppResources.NewPasswordWatermark;
         string passwordConfirmWatermark = AppResources.ConfirmNewPasswordWatermark;

        #endregion


        #region Constructors

        public PasswordUpdateControl()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            userApi = new UserApi();
            userApi.PasswordChangeCompleted += userApi_EnterPinCompleted;

            //// Fill in the password watermark
            //TextBoxPin.Text = pinWatermarkText;
            //TextBoxPin.Foreground = App.AppConstants.WatermarkTextColourBrush;

            //// Fill in the email watermark
            //TextBoxEmail.Text = emailWatermarkText;
            //TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxCurrentPasswordWatermark.Text = passwordCurrentWatermark;
            TextBoxCurrentPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxPasswordWatermark.Text = passwordWatermark;
            TextBoxPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxConfirmPasswordWatermark.Text = passwordConfirmWatermark;
            TextBoxConfirmPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            //if (App.ViewModel.DbViewModel.IsLoggedOn())
            //{
            //    TextBoxEmail.Text = App.ViewModel.DbViewModel.CurrentUser.Email;
            //    //TextBoxEmail.Foreground = App.AppConstants.NormalTextColourBrush;
            //    //TextBoxEmail.IsEnabled = false;

            //    App.AppConstants.SetNotEnabledTextBoxFocusColours(TextBoxEmail);

            //    TextBoxEmail.IsHitTestVisible = false;

            //    TextBlockLoggedIn.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    TextBlockLoggedIn.Visibility = System.Windows.Visibility.Collapsed;
            //}

            ResetErrors();
        }

        private void ResetErrors()
        {
            //TitleEmail.ClearError();
            //TitlePin.ClearError();
            TitleCurrentPassword.ClearError();
            TitlePassword.ClearError();
            TitleConfirmPassword.ClearError();
        }

        #endregion


        //#region Email Textbox

        ///// <summary>
        ///// The email box has got focus
        ///// </summary>
        //private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

        //    // if our watermark is set in the control
        //    if (TextBoxEmail.Text == emailWatermarkText)
        //    {
        //        TextBoxEmail.Text = string.Empty;
        //        TextBoxEmail.Foreground = App.AppConstants.NormalTextColourBrush;
        //    }
        //}

        ///// <summary>
        ///// The email box has lost focus
        ///// </summary>
        //private void TextBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    // If nothing has been entered on leaving our textbox, then set our watermark again
        //    if (TextBoxEmail.Text == string.Empty)
        //    {
        //        TextBoxEmail.Text = emailWatermarkText;
        //        TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;
        //    }
        //}

        ///// <summary>
        ///// The email textbox has received a key up
        ///// </summary>
        //private void TextBoxEmail_KeyUp(object sender, KeyEventArgs e)
        //{
        //    // Reset the errors if a key has been pressed
        //    if (e.Key != Key.Unknown)
        //    {
        //        ResetErrors();
        //    }

        //    // remove focus from the textbox when enter has been pressed
        //    if (e.Key == Key.Enter)
        //    {
        //        TextBoxPin.Focus();
        //    }
        //}

        //#endregion


        //#region Pin Textbox

        ///// <summary>
        ///// The Pin box has got focus
        ///// </summary>
        //private void TextBoxPin_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

        //    // if our watermark is set in the control
        //    if (TextBoxPin.Text == pinWatermarkText)
        //    {
        //        TextBoxPin.Text = string.Empty;
        //        TextBoxPin.Foreground = App.AppConstants.NormalTextColourBrush;
        //    }
        //}

        ///// <summary>
        ///// The Pin box has lost focus
        ///// </summary>
        //private void TextBoxPin_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    // If nothing has been entered on leaving our textbox, then set our watermark again
        //    if (TextBoxPin.Text == string.Empty)
        //    {
        //        TextBoxPin.Text = pinWatermarkText;
        //        TextBoxPin.Foreground = App.AppConstants.WatermarkTextColourBrush;
        //    }
        //}

        ///// <summary>
        ///// The email textbox has received a key up
        ///// </summary>
        //private void TextBoxPin_KeyUp(object sender, KeyEventArgs e)
        //{
        //    // Reset the errors if a key has been pressed
        //    if (e.Key != Key.Unknown)
        //    {
        //        ResetErrors();
        //    }

        //    // remove focus from the textbox when enter has been pressed
        //    if (e.Key == Key.Enter)
        //    {
        //        PasswordBoxUserPassword.Focus();
        //    }
        //}

        //#endregion


        #region Current Password Focus etc

        private void PasswordBoxCurrentPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as PasswordBox);

            // If the password box gets focus, turn off the watermark no matter what is in the password
            TextBoxCurrentPasswordWatermark.Opacity = 0;
            TextBoxCurrentPasswordWatermark.Background.Opacity = 0;

            // And turn on the password box
            PasswordBoxCurrentPassword.Opacity = 100;
        }

        private void PasswordBoxCurrentPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBoxCurrentPassword.Password);

            // If password empty, turn the password box off and display the watermark- if it has something in it we want to see the *'s in it
            PasswordBoxCurrentPassword.Opacity = passwordEmpty ? 0 : 100;

            // If password empty, turn the watermark on
            TextBoxCurrentPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            TextBoxCurrentPasswordWatermark.Background.Opacity = TextBoxCurrentPasswordWatermark.Opacity;
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void PasswordBoxCurrentPassword_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                PasswordBoxUserPassword.Focus();
            }
        }

        #endregion


        #region Password Focus etc

        private void PasswordBoxUserPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as PasswordBox);

            // If the password box gets focus, turn off the watermark no matter what is in the password
            TextBoxPasswordWatermark.Opacity = 0;
            TextBoxPasswordWatermark.Background.Opacity = 0;

            // And turn on the password box
            PasswordBoxUserPassword.Opacity = 100;
        }

        private void PasswordBoxUserPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBoxUserPassword.Password);

            // If password empty, turn the password box off and display the watermark- if it has something in it we want to see the *'s in it
            PasswordBoxUserPassword.Opacity = passwordEmpty ? 0 : 100;

            // If password empty, turn the watermark on
            TextBoxPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            TextBoxPasswordWatermark.Background.Opacity = TextBoxPasswordWatermark.Opacity;
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void PasswordBoxUserPassword_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                PasswordBoxConfirmUserPassword.Focus();
            }
        }

        #endregion


        #region Password ConfirmFocus etc

        private void PasswordBoxConfirmUserPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as PasswordBox);

            // If the password box gets focus, turn off the watermark no matter what is in the password
            TextBoxConfirmPasswordWatermark.Opacity = 0;
            TextBoxConfirmPasswordWatermark.Background.Opacity = 0;

            // And turn on the password box
            PasswordBoxConfirmUserPassword.Opacity = 100;
        }

        private void PasswordBoxConfirmUserPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBoxConfirmUserPassword.Password);

            // If password empty, turn the password box off and display the watermark- if it has something in it we want to see the *'s in it
            PasswordBoxConfirmUserPassword.Opacity = passwordEmpty ? 0 : 100;

            // If password empty, turn the watermark on
            TextBoxConfirmPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            TextBoxConfirmPasswordWatermark.Background.Opacity = TextBoxConfirmPasswordWatermark.Opacity;
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void PasswordBoxConfirmUserPassword_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                if (RemoveKeyboard != null)
                {
                    RemoveKeyboard(this, EventArgs.Empty);
                }
            }
        }
        #endregion


        #region Enter Pin Has Been Pressed

        /// <summary>
        /// The containing page will call login when the user is ready to login
        /// </summary>
        internal void EnterPin()
        {
            bool pageValid = false;

            ResetErrors();

            //if (Validation.ValidateExists(TitleEmail, TextBoxEmail, emailWatermarkText, "Please enter your registered email address"))
            //{
            //    if (Validation.EmailValid(TitleEmail, TextBoxEmail, emailWatermarkText, "Please enter a valid email address"))
            //    {
                    if (Validation.ValidateExists(TitleCurrentPassword, PasswordBoxCurrentPassword, passwordCurrentWatermark, AppResources.EnterCurrentPassword))
                    {
                        if (Validation.ValidateExists(TitlePassword, PasswordBoxUserPassword, passwordWatermark, AppResources.EnterNewPassword))
                        {
                            if (Validation.MinLength(TitlePassword, PasswordBoxUserPassword, passwordWatermark, AppResources.PasswordLength))
                            {
                                if (Validation.SamePassword(TitleConfirmPassword, PasswordBoxConfirmUserPassword, PasswordBoxUserPassword.Password, passwordConfirmWatermark, AppResources.PasswordMatch))
                                {
                                    pageValid = true;
                                }
                            }
                        }
                    }
            //    }
            //}

            // If page is valid then do the login
            if (pageValid)
            {
                //string email = TextBoxEmail.Text == emailWatermarkText ? string.Empty : TextBoxEmail.Text;
                //string pin = TextBoxPin.Text == pinWatermarkText ? string.Empty : TextBoxPin.Text;
                string currentPassword = PasswordBoxCurrentPassword.Password == passwordCurrentWatermark ? string.Empty : PasswordBoxCurrentPassword.Password;
                string newPassword = PasswordBoxUserPassword.Password == passwordWatermark ? string.Empty : PasswordBoxUserPassword.Password;

                // Raise message to containing page telling them we are starting a login
                if (PasswordUpdateStarting != null)
                {
                    PasswordUpdateStarting(this, new PasswordUpdateStartingEventArgs() { Count = 1 });
                }

                // Start the login
                userApi.PasswordChange(App.ViewModel.DbViewModel.CurrentUser.UserId, currentPassword, newPassword);
            }
        }

        #endregion


        #region Server Response Events

        void userApi_EnterPinCompleted(object sender, PasswordChangeEventArgs e)
        {
            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                // Raise message to containing page telling them we are completed a login
                if (PasswordUpdateCompleted != null)
                {
                    PasswordUpdateCompleted(this, new PasswordUpdateCompletedEventArgs() { Message = AppResources.PasswordChanged, Success = true });
                }
            }
            else if (e.ConnectionError == ApiConnectionResult.Bad)
            {
                if (PasswordUpdateCompleted != null)
                {
                    PasswordUpdateCompleted(this, new PasswordUpdateCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
            else
            {
                if (PasswordUpdateCompleted != null)
                {
                    PasswordUpdateCompleted(this, new PasswordUpdateCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
        }

        #endregion
    }
}
