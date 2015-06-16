#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WepApi;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    /// <summary>
    /// Events args returned when a favourite team selection changes
    /// </summary>
    public class TeamSelectorChangedStateEventArgs : EventArgs
    {
        public bool Enabled { get; set; }
    }



    public partial class RegisterControl
    {
        public class RegisterCompletedEventArgs : EventArgs
        {
            public bool Success { get; set; }

            public string Message { get; set; }
        }


        #region Events

        public event EventHandler<EventArgs> RegisterStarting;
        public event EventHandler<RegisterCompletedEventArgs> RegisterCompleted;

        public event EventHandler<EventArgs> FirstFavouriteTeamPressed;
        public event EventHandler<EventArgs> SecondFavouriteTeamPressed;
        public event EventHandler<EventArgs> MyCountryPressed;

        #endregion


        #region Fields

        readonly UserApi userApi;

        readonly string emailWatermark = AppResources.EmailWatermark;
        readonly string confirmEmailWatermark = AppResources.ConfirmEmailWatermark;
        readonly string passwordWatermark = AppResources.PasswordWatermark;
        readonly string passwordConfirmWatermark = AppResources.ConfirmPasswordWatermark;
        readonly string firstnameWatermark = AppResources.FirstnameWatermark;
        readonly string lastnameWatermark = AppResources.SurnameWatermark;
        readonly string mobileWatermark = AppResources.MobileWatermark;
        readonly string teamNameWatermark = "choose a team name";

        #endregion

        public static RegisterPage Page { get; set; }

        #region Constructors

        public RegisterControl()
        {
            InitializeComponent();

            // Initialise an instance of the user api
            userApi = new UserApi();
            userApi.RegisterCompleted += userApi_RegisterCompleted;

            TextBoxEmail.Text = emailWatermark;
            TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxConfirmEmail.Text = confirmEmailWatermark;
            TextBoxConfirmEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxPasswordWatermark.Text = passwordWatermark;
            TextBoxPasswordWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxPasswordConfirmWatermark.Text = passwordConfirmWatermark;
            TextBoxPasswordConfirmWatermark.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxFirstName.Text = firstnameWatermark;
            TextBoxFirstName.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxLastName.Text = lastnameWatermark;
            TextBoxLastName.Foreground = App.AppConstants.WatermarkTextColourBrush;

            TextBoxMobile.Text = mobileWatermark;
            TextBoxMobile.Foreground = App.AppConstants.WatermarkTextColourBrush;

            //TextBoxTeamName.Text = teamNameWatermark;
            //TextBoxTeamName.Foreground = App.AppConstants.WatermarkTextColourBrush;

            // Set the titles
            ResetErrors();

            // Subscribe to the pressed event of the team selectors
            FavouriteTeam1.TeamPressed += FavouriteTeam1_TeamPressed;
            FavouriteTeam1.NoSelectionMadeText = AppResources.FavouriteTeam1;
            FavouriteTeam2.NoSelectionMadeText = AppResources.FavouriteTeam2;

            FavouriteTeam2.TeamPressed += FavouriteTeam2_TeamPressed;
            MyCountry.CountryPressed += MyCountry_CountryPressed;

            HyperlinkTC.Click += HyperlinkTC_Click;
            HyperlinkPrivacy.Click += HyperlinkPrivacy_Click;
        }

        #endregion


        #region Navigation

        void HyperlinkTC_Click(object sender, RoutedEventArgs e)
        {
            Page.NavigationService.Navigate(new Uri("/Views/BrowsePage.xaml?url=http://cdn.moshen.com/fas/terms.html", UriKind.Relative));
        }

        void HyperlinkPrivacy_Click(object sender, RoutedEventArgs e)
        {
            Page.NavigationService.Navigate(new Uri("/Views/BrowsePage.xaml?url=http://cdn.moshen.com/fas/privacy.html", UriKind.Relative));
        }

        #endregion


        #region Favourite Team

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
            if ( MyCountryPressed != null )
            {
                MyCountryPressed(this, new EventArgs());
            }
        }

        #endregion


        #region Email Textbox

        /// <summary>
        /// The email box has got focus
        /// </summary>
        private void TextBoxEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxEmail.Text == emailWatermark)
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
                TextBoxEmail.Text = emailWatermark;
                TextBoxEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxEmail_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                TextBoxConfirmEmail.Focus();
            }
        }

        #endregion


        #region Confirm Email Textbox

        /// <summary>
        /// The email box has got focus
        /// </summary>
        private void TextBoxConfirmEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            // if our watermark is set in the control
            if (TextBoxConfirmEmail.Text == confirmEmailWatermark)
            {
                TextBoxConfirmEmail.Text = string.Empty;
                TextBoxConfirmEmail.Foreground = App.AppConstants.NormalTextColourBrush;
            }
        }

        /// <summary>
        /// The email box has lost focus
        /// </summary>
        private void TextBoxConfirmEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            // If nothing has been entered on leaving our textbox, then set our watermark again
            if (TextBoxConfirmEmail.Text == string.Empty)
            {
                TextBoxConfirmEmail.Text = confirmEmailWatermark;
                TextBoxConfirmEmail.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        /// <summary>
        /// Also validate on a key up
        /// </summary>
        private void TextBoxConfirmEmail_KeyUp(object sender, KeyEventArgs e)
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
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                PasswordBoxUserPasswordConfirm.Focus();
            }
        }

        #endregion


        #region Password ConfirmFocus etc

        private void PasswordBoxUserPasswordConfirm_GotFocus(object sender, RoutedEventArgs e)
        {
            // If the password box gets focus, turn off the watermark no matter what is in the password
            TextBoxPasswordConfirmWatermark.Opacity = 0;
            TextBoxPasswordConfirmWatermark.Background.Opacity = 0;

            // And turn on the password box
            PasswordBoxUserPasswordConfirm.Opacity = 100;
        }

        private void PasswordBoxUserPasswordConfirm_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordBoxUserPasswordConfirm.Password);

            // If password empty, turn the password box off and display the watermark- if it has something in it we want to see the *'s in it
            PasswordBoxUserPasswordConfirm.Opacity = passwordEmpty ? 0 : 100;

            // If password empty, turn the watermark on
            TextBoxPasswordConfirmWatermark.Opacity = passwordEmpty ? 100 : 0;
            TextBoxPasswordConfirmWatermark.Background.Opacity = TextBoxPasswordConfirmWatermark.Opacity;
        }

        /// <summary>
        /// The email textbox has received a key up
        /// </summary>
        private void PasswordBoxUserPasswordConfirm_KeyUp(object sender, KeyEventArgs e)
        {
            ResetErrors();

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                TextBoxFirstName.Focus();
            }
        }

        #endregion


        #region First Name Focus Handlers

        private void TextBoxFirstName_GotFocus(object sender, RoutedEventArgs e)
        {
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
        /// The first name textbox has received a key up
        /// </summary>
        private void TextBoxFirstName_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                TextBoxLastName.Focus();
                // and scroll down
                Page.RegisterScrollViewer.ScrollToVerticalOffset(90);
            }
        }

        #endregion


        #region Last Name Focus Handlers

        private void TextBoxLastName_GotFocus(object sender, RoutedEventArgs e)
        {
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
        /// The last name textbox has received a key up
        /// </summary>
        private void TextBoxLastName_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                // get rid of the keyboard
                Page.Focus();
                // and scroll to the fav team
                Page.RegisterScrollViewer.ScrollToVerticalOffset(455);
            }
        }

        #endregion


        #region Mobile Phone Focus Handlers

        private void TextBoxMobile_GotFocus(object sender, RoutedEventArgs e)
        {
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
        /// The mobile textbox has received a key up
        /// </summary>
        private void TextBoxMobile_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the errors if a key has been pressed
            if (e.Key != Key.Unknown)
            {
                ResetErrors();
            }

            // remove focus from the textbox when enter has been pressed
            if (e.Key == Key.Enter)
            {
                // there is no enter key on the mobile
            }
        }

        #endregion


        public void ResetErrors()
        {
            TitleEmail.ClearError();
            TitleConfirmEmail.ClearError();
            TitlePassword.ClearError();
            TitlePasswordConfirm.ClearError();
            TitleFirstName.ClearError();
            TitleLastName.ClearError();
            TitleMobile.ClearError();
            TitleFavTeam1.ClearError();
            TitleFavTeam2.ClearError();
            TitleMyCountry.ClearError();
            TitleAcceptTC.ClearError();
        }


        #region Regsitration Events

        public void Register(ScrollViewer scroller)
        {
            bool pageValid = false;
            int scrollerOffset = 0;

            Page.Focus();

            ResetErrors();

            if (Validation.ValidateExists(TitleEmail, TextBoxEmail, emailWatermark, "Please enter your email address"))
            {
                if (Validation.EmailValid(TitleEmail, TextBoxEmail, emailWatermark, "Please enter a valid email address"))
                {
                    if (Validation.SameEmail(TitleConfirmEmail, TextBoxConfirmEmail, TextBoxEmail.Text, emailWatermark, "Please confirm your email address"))
                    {
                        if (Validation.ValidateExists(TitlePassword, PasswordBoxUserPassword, passwordWatermark, "Please enter your password"))
                        {
                            if (Validation.MinLength(TitlePassword, PasswordBoxUserPassword, passwordWatermark, "At least 6 characters"))
                            {
                                if (Validation.SamePassword(TitlePasswordConfirm, PasswordBoxUserPasswordConfirm, PasswordBoxUserPassword.Password, passwordConfirmWatermark, "Passwords must match"))
                                {
                                    if (Validation.ValidateExists(TitleFirstName, TextBoxFirstName, firstnameWatermark, "Please enter your first name"))
                                    {
                                        scrollerOffset = 90; // move down 90 for last name

                                        if (Validation.ValidateExists(TitleLastName, TextBoxLastName, lastnameWatermark, "Please enter your last name"))
                                        {
                                            scrollerOffset = 445; // move down 445 for anything further down

                                            // remove the keyboard from last name
                                            Page.Focus();
                                            if (Validation.ValidateSelectorChosen(TitleFavTeam1, FavouriteTeam1.SelectedId(), "Please select a favourite club"))
                                            {
                                                if (Validation.ValidateSelectorChosen(TitleFavTeam2, FavouriteTeam2.SelectedId(), "Please select a second favourite club"))
                                                {
                                                    if (Validation.ValidateSelectorChosen(TitleMyCountry, MyCountry.SelectedId(), "Please select a country"))
                                                    {
                                                        if (Validation.ValidateExists(TitleMobile, TextBoxMobile, mobileWatermark, "Please enter your mobile number"))
                                                        {
                                                            if (CheckBoxTC.IsChecked == true)
                                                            {
                                                                pageValid = true;
                                                            }
                                                            else
                                                            {
                                                                MessageBox.Show("Please accept the terms and conditions", "Terms and conditions", MessageBoxButton.OK);
                                                                scrollerOffset = 500; 
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // This control is in a scroller - so put the user at teh correct place if there is an error
            if (!pageValid)
            {
                scroller.ScrollToVerticalOffset(scrollerOffset);
            }

            //string email = TextBoxEmail.Text == emailWatermark ? string.Empty : TextBoxEmail.Text;
            //string password = PasswordBoxUserPassword.Password == passwordWatermark ? string.Empty : PasswordBoxUserPassword.Password;
            //string firstname = TextBoxFirstName.Text == firstnameWatermark ? string.Empty : TextBoxFirstName.Text;
            //string lastname = TextBoxLastName.Text == lastnameWatermark ? string.Empty : TextBoxLastName.Text;
            //string mobile = TextBoxMobile.Text == mobileWatermark ? string.Empty : TextBoxMobile.Text;
            //string teamname = TextBoxTeamName.Text == teamNameWatermark ? string.Empty : TextBoxTeamName.Text;

            if ( pageValid )
            {
                string fav1 = FavouriteTeam1.SelectedId();
                string fav2 = FavouriteTeam2.SelectedId();
                CountryRecord country = MyCountry.Country;
                string terms = CheckBoxTC.IsChecked == true ? "on" : "off";

                // Raise message to containing page telling them we are starting a login
                if (RegisterStarting != null)
                {
                    RegisterStarting(this, new LoginStartingEventArgs() { Count = 1 });
                }

                userApi.Register(TextBoxEmail.Text, PasswordBoxUserPassword.Password, TextBoxFirstName.Text, TextBoxLastName.Text, TextBoxMobile.Text, fav1, fav2, string.Empty, country == null ? "0" : country.CountryId.ToString(), terms);
            }
        }


        void userApi_RegisterCompleted(object sender, RegisterEventArgs e)
        {
            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // Log the user on
                App.ViewModel.DbViewModel.Login(e.ServerResponse.Details);

                // Raise message to containing page telling them we are completed a login
                if (RegisterCompleted != null)
                {
                    RegisterCompleted(this, new RegisterCompletedEventArgs() { Message = "Please verify your account by entering the pin sent to your email address.", Success = true });
                }
            }
            else if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Bad)
            {
                // Log them off just to be sure
                App.ViewModel.DbViewModel.Logout();

                if (RegisterCompleted != null)
                {
                    RegisterCompleted(this, new RegisterCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
            else
            {
                // Log them off just to be sure
                App.ViewModel.DbViewModel.Logout();

                if (RegisterCompleted != null)
                {
                    RegisterCompleted(this, new RegisterCompletedEventArgs() { Message = e.FriendlyMessage, Success = false });
                }
            }
        }

        #endregion


        internal void SetTeamsAndCountry(ClubRecord fav1, ClubRecord fav2, CountryRecord country)
        {
            FavouriteTeam1.Refresh(fav1);
            FavouriteTeam2.Refresh(fav2);
            MyCountry.Refresh(country);
        }
    }
}
