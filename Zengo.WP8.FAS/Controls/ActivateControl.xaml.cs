
#region Usings

using Moshen.WP7.FAS.Resources;
using Moshen.WP7.FAS.WebApi.Responses;
using Moshen.WP7.FAS.WepApi;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace Moshen.WP7.FAS.Controls
{
    public partial class ActivateControl : UserControl
    {
        #region Event Args

        public class ActivateStartingEventArgs : EventArgs
        {
            public int Count { get; set; }
        }

        public class ActivateCompletedEventArgs : EventArgs
        {
            public bool Success { get; set; }

            public string Message { get; set; }
        }

        #endregion


        #region Events

        public event EventHandler<ActivateStartingEventArgs> ActivateStarting;
        public event EventHandler<ActivateCompletedEventArgs> ActivateCompleted;
        public event EventHandler<EventArgs> RemoveKeyboard;
        public event EventHandler<EventArgs> ResendPin;

        #endregion


        #region Fields

        UserApi userApi;

        string pinWatermarkText = AppResources.EnterPinWatermark;

        #endregion


        #region Constructors

        public ActivateControl()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            userApi = new UserApi();
            userApi.VerifyCompleted += userApi_VerifyCompleted;

            // Fill in the password watermark
            TextBoxPin.Text = pinWatermarkText;
            TextBoxPin.Foreground = App.AppConstants.WatermarkTextColourBrush;

            //// Fill in the email watermark
            //TextBoxEmail.Text = emailWatermarkText;
            //TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            //if (App.ViewModel.DbViewModel.CurrentUser != null)
            //{
            //    TextBoxEmail.Text = App.ViewModel.DbViewModel.CurrentUser.Email;
            //    TextBoxEmail.Foreground = App.NormalTextColourBrush;
            //}

            // Reset the error messages
            ResetErrors();
        }

        #endregion


        #region Email Textbox

        ///// <summary>
        ///// The email box has got focus
        ///// </summary>
        //private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    // if our watermark is set in the control
        //    if (TextBoxEmail.Text == emailWatermarkText)
        //    {
        //        TextBoxEmail.Text = string.Empty;
        //        TextBoxEmail.Foreground = App.NormalTextColourBrush;
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
        //    GridError.Visibility = System.Windows.Visibility.Collapsed;

        //    // remove focus from the textbox when enter has been pressed
        //    if (e.Key == Key.Enter)
        //    {
        //        if (RemoveKeyboard != null)
        //        {
        //            RemoveKeyboard(this, EventArgs.Empty);
        //        }
        //    }
        //}

        #endregion


        #region Pin Textbox

        /// <summary>
        /// The Pin box has got focus
        /// </summary>
        private void TextBoxPin_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxPin.Text == pinWatermarkText)
            {
                TextBoxPin.Text = string.Empty;
                TextBoxPin.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The Pin box has lost focus
        /// </summary>
        private void TextBoxPin_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxPin.Text == string.Empty)
            {
                TextBoxPin.Text = pinWatermarkText;
                TextBoxPin.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void TextBoxPin_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

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
        /// The containing page will call activate when the user presses the tick
        /// </summary>
        internal void Activate()
        {
            bool pageValid = false;

            ResetErrors();
            
            if (Validation.ValidateExists(TitlePin, TextBoxPin, pinWatermarkText, AppResources.EnterPinFromEmail))
            {
                pageValid = true;
            }

            if (pageValid)
            {
                string pin = TextBoxPin.Text == pinWatermarkText ? string.Empty : TextBoxPin.Text;

                // Raise message to containing page telling them we are starting a login
                if (ActivateStarting != null)
                {
                    ActivateStarting(this, new ActivateStartingEventArgs() { Count = 1 });
                }

                // Start the activation
                userApi.Verify(App.ViewModel.DbViewModel.CurrentUser.UserId, pin);
            }
        }

        #endregion

        private void ResetErrors()
        {
            TitlePin.ClearError();
        }


        #region Server Response Events

        void userApi_VerifyCompleted(object sender, VerifyEventArgs e)
        {
            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                App.ViewModel.DbViewModel.Validate(e.ServerResponse.Details.User.id);

                // Raise message to containing page telling them we are completed a login
                if (ActivateCompleted != null)
                {
                    ActivateCompleted(this, new ActivateCompletedEventArgs() { Message = AppResources.ActivationCompleted, Success = true });
                }
            }
            else if (e.ConnectionError == ApiConnectionResult.Bad)
            {
                if (ActivateCompleted != null)
                {
                    ActivateCompleted(this, new ActivateCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
            else
            {
                if (ActivateCompleted != null)
                {
                    ActivateCompleted(this, new ActivateCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
        }

        #endregion

        private void HyperlinkResendPin_Click_1(object sender, RoutedEventArgs e)
        {
            if (ResendPin != null)
            {
                ResendPin(this, new EventArgs());
            }
        }
    }
}
