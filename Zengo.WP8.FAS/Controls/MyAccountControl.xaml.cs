
#region Usings

using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public class UpdateAccountStartingEventArgs : EventArgs
    {
        public int Count { get; set; }
    }

    public class UpdateAccountCompletedEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public partial class MyAccountControl : UserControl
    {
        public class ErrorAndControl
        {
            public FieldTitleAndError Error { get; set; }
            public UIElement Element { get; set; }
        }

        #region Events

        public event EventHandler<EventArgs> FirstFavouriteTeamPressed;
        public event EventHandler<EventArgs> SecondFavouriteTeamPressed;
        public event EventHandler<EventArgs> MyCountryPressed;

        public event EventHandler<UpdateAccountStartingEventArgs> UpdateAccountStarting;
        public event EventHandler<UpdateAccountCompletedEventArgs> UpdateAccountCompleted;

        #endregion


        #region Fields

        UserApi userApi;

        string firstnameWatermark = AppResources.FirstnameWatermark;
        string lastnameWatermark = AppResources.SurnameWatermark;
        string mobileWatermark = AppResources.MobileWatermark;

        List<ErrorAndControl> errors;

        #endregion


        #region Constructors

        public MyAccountControl()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            userApi = new UserApi();
            userApi.UpdateCompleted += userApi_UpdateCompleted;

            // Subscribe to the pressed event of the team selectors
            FavouriteTeam1.TeamPressed += FavouriteTeam1_TeamPressed;
            FavouriteTeam2.TeamPressed += FavouriteTeam2_TeamPressed;
            MyCountry.CountryPressed += MyCountry_CountryPressed;

            // Put all of the error controls in a list
            errors = new List<ErrorAndControl>();
            int index = 0;
            foreach (object control in LayoutRoot.Children)
            {
                index += 1;
                if (control is FieldTitleAndError)
                {
                    ErrorAndControl errorAndControl = new ErrorAndControl() { Error = control as FieldTitleAndError, Element = LayoutRoot.Children[index] };

                    errors.Add(errorAndControl);
                }
            }

            // Set the titles
            ResetErrors();
        }

        #endregion


        #region EventHandlers

        /// <summary>
        /// They have clicked on the favourite team control ("Please choose a favourite team")
        /// </summary>
        void FavouriteTeam1_TeamPressed(object sender, EventArgs e)
        {
            if (FirstFavouriteTeamPressed != null)
            {
                FirstFavouriteTeamPressed(this, new EventArgs());
            }
        }

        void FavouriteTeam2_TeamPressed(object sender, EventArgs e)
        {
            if (SecondFavouriteTeamPressed != null)
            {
                SecondFavouriteTeamPressed(this, new EventArgs());
            }
        }

        void MyCountry_CountryPressed(object sender, EventArgs e)
        {
            if (MyCountryPressed != null)
            {
                MyCountryPressed(this, new EventArgs());
            }
        }

        void userApi_UpdateCompleted(object sender, UpdateEventArgs e)
        {
            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                // Update the users details in the database
                App.ViewModel.DbViewModel.UpdateUser(e.ServerResponse.Details);

                // Raise message to containing page telling them we are completed a login
                if (UpdateAccountCompleted != null)
                {
                    UpdateAccountCompleted(this, new UpdateAccountCompletedEventArgs() { Message = AppResources.UpdateAccountSuccess, Success = true });
                }
            }
            else if (e.ConnectionError == ApiConnectionResult.Bad)
            {
                if (UpdateAccountCompleted != null)
                {
                    UpdateAccountCompleted(this, new UpdateAccountCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
            else
            {
                if (UpdateAccountCompleted != null)
                {
                    UpdateAccountCompleted(this, new UpdateAccountCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }

            e.ShowDebugMessage();
        }

        internal void SetFavouriteTeamFromHiddenId(ClubRecord team1, ClubRecord team2, CountryRecord myCountry)
        {
            FavouriteTeam1.Refresh(team1);

            FavouriteTeam2.Refresh(team2);

            MyCountry.Refresh(myCountry);
        }

        #endregion


        #region This Controls Event Handlers

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxFirstName.Text))
            {
                TextBoxFirstName.Text = firstnameWatermark;
                TextBoxFirstName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

            if (string.IsNullOrEmpty(TextBoxLastName.Text))
            {
                TextBoxLastName.Text = lastnameWatermark;
                TextBoxLastName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

            if (string.IsNullOrEmpty(TextBoxMobile.Text))
            {
                TextBoxMobile.Text = mobileWatermark;
                TextBoxMobile.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

            //if (string.IsNullOrEmpty(TextBoxTeamName.Text))
            //{
            //    TextBoxTeamName.Text = teamNameWatermark;
            //    TextBoxTeamName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            //}
        }

        #endregion


        #region First Name Focus Handlers

        private void TextBoxFirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

            // if our watermark is set in the control
            if (TextBoxFirstName.Text == firstnameWatermark)
            {
                TextBoxFirstName.Text = string.Empty;
                TextBoxFirstName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        private void TextBoxFirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxFirstName.Text == string.Empty)
            {
                TextBoxFirstName.Text = firstnameWatermark;
                TextBoxFirstName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxFirstName_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed and go to the next field
            if (e.Key == Key.Enter)
            {
                TextBoxLastName.Focus();
            }
        }

        #endregion


        #region Last Name Focus Handlers

        private void TextBoxLastName_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

            // if our watermark is set in the control
            if (TextBoxLastName.Text == lastnameWatermark)
            {
                TextBoxLastName.Text = string.Empty;
                TextBoxLastName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        private void TextBoxLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxLastName.Text == string.Empty)
            {
                TextBoxLastName.Text = lastnameWatermark;
                TextBoxLastName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxLastName_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed and go to the next field
            if (e.Key == Key.Enter)
            {
                TextBoxMobile.Focus();
            }
        }

        #endregion


        #region Mobile Focus Handlers

        private void TextBoxMobile_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

            // if our watermark is set in the control
            if (TextBoxMobile.Text == mobileWatermark)
            {
                TextBoxMobile.Text = string.Empty;
                TextBoxMobile.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        private void TextBoxMobile_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxMobile.Text == string.Empty)
            {
                TextBoxMobile.Text = mobileWatermark;
                TextBoxMobile.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxMobile_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed and go to the next field
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }

        #endregion


        #region Team Name Focus Handlers

        //private void TextBoxTeamName_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    // if our watermark is set in the control
        //    if (TextBoxTeamName.Text == teamNameWatermark)
        //    {
        //        TextBoxTeamName.Text = string.Empty;
        //        TextBoxTeamName.Foreground = App.AppConstants.NormalTextColourBrush;
        //    }
        //}

        //private void TextBoxTeamName_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    // If nothing has been entered on leaving our textbox, then set our watermark again
        //    if (TextBoxTeamName.Text == string.Empty)
        //    {
        //        TextBoxTeamName.Text = teamNameWatermark;
        //        TextBoxTeamName.Foreground = App.AppConstants.WatermarkTextColourBrush;
        //    }
        //}

        #endregion


        private void ResetErrors()
        {
            foreach (ErrorAndControl title in errors)
            {
                title.Error.ClearError();
            }
        }

        /// <summary>
        /// Instruction from the containing page to save (update account)
        /// </summary>
        internal void Save(ScrollViewer scroller)
        {
            bool pageValid = false;
            int scrollerOffset = 0;

            ResetErrors();

            // Do some validation
            if (Validation.ValidateExists(TitleFirstName, TextBoxFirstName, firstnameWatermark, AppResources.FirstnameWatermark))
            {
                scrollerOffset = 90;
                if (Validation.ValidateExists(TitleLastName, TextBoxLastName, lastnameWatermark, AppResources.SurnameWatermark))
                {
                    scrollerOffset = 180;
                    if (Validation.ValidateExists(TitleMobile, TextBoxMobile, mobileWatermark, AppResources.MobileWatermark))
                    {
                        pageValid = true;
                    }
                }
            }

            // This control is in a scroller - so put the user at teh correct place if there is an error
            if ( !pageValid )
            {
                scroller.ScrollToVerticalOffset(scrollerOffset);
            }

            // If page is valid then call the api
            if (pageValid)
            {
                string firstname = TextBoxFirstName.Text == firstnameWatermark ? string.Empty : TextBoxFirstName.Text;
                string lastname = TextBoxLastName.Text == lastnameWatermark ? string.Empty : TextBoxLastName.Text;
                string mobile = TextBoxMobile.Text == mobileWatermark ? string.Empty : TextBoxMobile.Text;
                //string teamname = TextBoxTeamName.Text == teamNameWatermark ? string.Empty : TextBoxTeamName.Text;
                string fav1 = FavouriteTeam1.SelectedId();
                string fav2 = FavouriteTeam2.SelectedId();
                string country = MyCountry.SelectedId();

                // Raise message to containing page telling them we are starting a login so they can display progress bar etc
                if (UpdateAccountStarting != null)
                {
                    UpdateAccountStarting(this, new UpdateAccountStartingEventArgs() { Count = 1 });
                }

                // Call the api
                userApi.UserUpdate(App.ViewModel.DbViewModel.CurrentUser.UserId, firstname, lastname, mobile, fav1, fav2, country);
            }


        }
    }
}
