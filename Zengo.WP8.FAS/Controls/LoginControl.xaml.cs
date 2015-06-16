
#region Usings

using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public class LoginStartingEventArgs : EventArgs
    {
        public int Count { get; set; }
    }

    public class LoginCompletedEventArgs : EventArgs
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public partial class LoginControl : UserControl
    {
        #region Events

        public event EventHandler<LoginStartingEventArgs> LoginStarting;
        public event EventHandler<LoginCompletedEventArgs> LoginCompleted;
        public event EventHandler<EventArgs> RemoveKeyboard;

        #endregion


        #region Fields

        UserApi userApi;

        string emailWatermarkText = AppResources.EmailWatermark;
        string passwordWatermarkText = AppResources.PasswordWatermark;

        #endregion


        #region Constructors

        public LoginControl()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            userApi = new UserApi();
            userApi.LoginCompleted += userApi_LoginCompleted;

            // Fill in the password watermark
            TextBoxPasswordWatermark.Text = passwordWatermarkText;
            TextBoxPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            // Fill in the email watermark
            TextBoxEmail.Text = emailWatermarkText;
            TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            // Set the titles
            TitleEmail.ClearError();
            TitlePassword.ClearError();
        }

        #endregion


        #region Email Textbox

        /// <summary>
        /// The email box has got focus
        /// </summary>
        private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

            // if our watermark is set in the control
            if (TextBoxEmail.Text == emailWatermarkText)
            {
                TextBoxEmail.Text = string.Empty;
                TextBoxEmail.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The email box has lost focus
        /// </summary>
        private void TextBoxEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxEmail.Text == string.Empty)
            {
                TextBoxEmail.Text = emailWatermarkText;
                TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void TextBoxEmail_KeyUp(object sender, KeyEventArgs e)
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
                if (RemoveKeyboard != null)
                {
                    RemoveKeyboard(this, EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Login Has Been Pressed

        /// <summary>
        /// The containing page will call login when the user is ready to login
        /// </summary>
        internal void Login()
        {
            bool pageValid = false;

            ResetErrors();

            // show the update entry
            if (TextBoxEmail.Text == "fas@fas.com" && PasswordBoxUserPassword.Password == "fas")
            {
                App.AppConstants.EnableUpdatesMenu ^= true;
            }

            if (Validation.ValidateExists(TitleEmail, TextBoxEmail, emailWatermarkText, "Please enter your email address"))
            {
                if (Validation.EmailValid(TitleEmail, TextBoxEmail, emailWatermarkText, "Please enter a valid email address"))
                {
                    if (Validation.ValidateExists(TitlePassword, PasswordBoxUserPassword, passwordWatermarkText, "Please enter your password"))
                    {
                        pageValid = true;
                    }
                }
            }

            // If page is valid then do the login
            if (pageValid)
            {
                // Raise message to containing page telling them we are starting a login
                if (LoginStarting != null)
                {
                    LoginStarting(this, new LoginStartingEventArgs() { Count = 1 });
                }

                // Start the login
                userApi.LoginV2(TextBoxEmail.Text, PasswordBoxUserPassword.Password);
            }
        }

        #endregion



        private void ResetErrors()
        {
            TitleEmail.ClearError();
            TitlePassword.ClearError();
        }


        #region Server Response Events

        void userApi_LoginCompleted(object sender, LoginEventArgs e)
        {
            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // Log the user on
                App.ViewModel.DbViewModel.Login(e.ServerResponse.Details);

                // Raise message to containing page telling them we are completed a login
                if (LoginCompleted != null)
                {
                    LoginCompleted(this, new LoginCompletedEventArgs() { Message = "Login successful.", Success = true });
                }
            }
            else if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Bad)
            {
                // eg username and password dont match

                // Log them off just to be sure
                //App.ViewModel.DbViewModel.Logout();

                if (LoginCompleted != null)
                {
                    LoginCompleted(this, new LoginCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
            else if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Aborted)
            {
            }
            else
            {
                // eg a problem

                // Log them off just to be sure
                //App.ViewModel.DbViewModel.Logout();

                if (LoginCompleted != null)
                {
                    LoginCompleted(this, new LoginCompletedEventArgs() { Message = "Please check your internet connection", Success = false });
                }
            }

            e.ShowDebugMessage();
        }

        #endregion
    }
}
