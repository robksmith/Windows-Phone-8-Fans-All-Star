
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
    public class PasswordResetStartingEventArgs : EventArgs
    {
        public int Count { get; set; }
    }

    public class PasswordResetCompletedEventArgs : EventArgs
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }

    public partial class PasswordResetControl : UserControl
    {
        #region Events

        public event EventHandler<PasswordResetStartingEventArgs> PasswordResetStarting;
        public event EventHandler<PasswordResetCompletedEventArgs> PasswordResetCompleted;
        public event EventHandler<EventArgs> RemoveKeyboard;

        #endregion


        #region Fields

        UserApi userApi;

        readonly string emailWatermarkText = AppResources.ResetEmailWatermark;

        #endregion


        #region Constructors

        public PasswordResetControl()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            userApi = new UserApi();
            userApi.PasswordResetCompleted += userApi_SendPinCompleted;

            // Fill in the email watermark
            TextBoxEmail.Text = emailWatermarkText;
            TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            if (App.ViewModel.DbViewModel.IsLoggedOn())
            {
                TextBoxEmail.Text = App.ViewModel.DbViewModel.CurrentUser.Email;

                App.AppConstants.SetNotEnabledTextBoxFocusColours(TextBoxEmail);

                TextBoxEmail.IsHitTestVisible = false;

                TextBlockLoggedIn.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TextBlockLoggedIn.Visibility = System.Windows.Visibility.Collapsed;
            }

            // Clear the errors
            TitleEmail.ClearError();

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
            TitleEmail.ClearError();

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


        #region RequestPin Has Been Pressed

        /// <summary>
        /// The containing page will call send pin when the user is ready to (pressed the tick)
        /// </summary>
        internal void RequestPin()
        {
            bool pageValid = false;

            string email = TextBoxEmail.Text == emailWatermarkText ? string.Empty : TextBoxEmail.Text;

            TitleEmail.ClearError();

            if (Validation.ValidateExists(TitleEmail, TextBoxEmail, emailWatermarkText,AppResources.ResetEmailWatermark))
            {
                if (Validation.EmailValid(TitleEmail, TextBoxEmail, emailWatermarkText, AppResources.EnterValidEmail))
                {
                    pageValid = true;
                }
            }

            // If page is valid then do the work
            if (pageValid)
            {
                // Raise message to containing page telling them we are starting a login
                if (PasswordResetStarting != null)
                {
                    PasswordResetStarting(this, new PasswordResetStartingEventArgs() { Count = 1 });
                }

                // Start the login
                userApi.PasswordReset(email);
            }
        }

        #endregion


        #region Server Response Events

        void userApi_SendPinCompleted(object sender, PasswordResetEventArgs e)
        {
            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                // Raise message to containing page telling them we are completed a login
                if (PasswordResetCompleted != null)
                {
                    PasswordResetCompleted(this, new PasswordResetCompletedEventArgs() { Message = AppResources.NewPasswordMessage, Success = true });
                }
            }
            else if (e.ConnectionError == ApiConnectionResult.Bad)
            {
                if (PasswordResetCompleted != null)
                {
                    PasswordResetCompleted(this, new PasswordResetCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
            else
            {
                if (PasswordResetCompleted != null)
                {
                    PasswordResetCompleted(this, new PasswordResetCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
        }

        #endregion

    }
}
