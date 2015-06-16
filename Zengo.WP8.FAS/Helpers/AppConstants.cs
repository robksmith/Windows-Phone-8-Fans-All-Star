
#region Usings

using Zengo.WP8.FAS.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class AppConstants
    {
        #region Fields

        // Do we need to be logged in before we can navigate from the home page
        private bool requireLogonToNavigateFromHomePage = true;

        // Instead of calling the vote api, call the players api to pass time and then add a vote to the db
        private bool simulateVote = false;

        // Do we want updates menu entry turned on in the main page
        private bool enableUpdatesMenu = false;

        // How many seconds to hold the splash screen
        private int secondsToHoldSplashScreen = 2;

        // When we cast a vote, we mark it as pending so another attempted cast cannot grab it. Then for this set number of seconds, no one else can grab it to use the vote. After this set number of seconds, we assume the vote has failed and it can be grabbed and used again.
        private int secondsToAllowAPendingVote = 45;

        // The manager names
        private string europeManager = "Capello";
        private string rotwManager = "Scolari";

        public bool ApplyingUpdates { get; set; }

        #endregion


        #region Properties

        public int SecondsToHoldSplashScreen { get { return secondsToHoldSplashScreen; } private set { } }

        public int SecondsToAllowAPendingVote { get { return secondsToAllowAPendingVote; } private set { } }

        public bool RequireLogonToNavigateFromHomePage { get { return requireLogonToNavigateFromHomePage; } private set { } }

        public bool SimulateVote { get { return simulateVote; } private set { } }

        public bool EnableUpdatesMenu { get { return enableUpdatesMenu; } set { enableUpdatesMenu = value; } }

        public string EuropeManager { get { return europeManager; } private set { } }

        public string RotwManager { get { return rotwManager; } private set { } }


        // The text watermark colours
        public SolidColorBrush NormalTextColourBrush { get; set; }
        public SolidColorBrush WatermarkTextColourBrush { get; set; }

        public SolidColorBrush FocusTextBoxBackgroundBrush { get; set; }
        public SolidColorBrush FocusTextBoxBorderBrush { get; set; }

        public SolidColorBrush FocusNotEnabledTextBoxBackgroundBrush { get; set; }
        public SolidColorBrush FocusNotEnabledTextBoxBorderBrush { get; set; }

        // Temporary location for fav teams and my country
        public ClubRecord FirstFavTeam { get; set; }
        public ClubRecord SecondFavTeam { get; set; }
        public CountryRecord MyCountry { get; set; }
        public PlayerRecord FavPlayer { get; set; }
        public LeagueAnswer FavLeague { get; set; }
        public StadiumAnswer FavStadium { get; set; }
        public SportAnswer FavSport { get; set; }

        // show various debug messages
        public bool ShowUserDataPollMessages { get; set; }
        public bool ShowGameDataPollMessages { get; set; }
        

        #endregion


        #region Constructors

        public AppConstants()
        {
#if DEBUG
            enableUpdatesMenu = true;
            requireLogonToNavigateFromHomePage = false;
#endif

            FirstFavTeam = null;
            SecondFavTeam = null;
            MyCountry = null;

            ShowUserDataPollMessages = false;
            ShowGameDataPollMessages = false;

            NormalTextColourBrush = new SolidColorBrush(Colors.Black);
            WatermarkTextColourBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xbf, 0xbf, 0xbf));

            FocusTextBoxBackgroundBrush = new SolidColorBrush(Colors.White);
            FocusTextBoxBorderBrush = (SolidColorBrush)Application.Current.Resources["TextBoxBorderBrush"];

            FocusNotEnabledTextBoxBackgroundBrush = new SolidColorBrush(Colors.Gray);
            FocusNotEnabledTextBoxBorderBrush = (SolidColorBrush)Application.Current.Resources["TextBoxBorderBrush"];
        }


        public void SetTextBoxFocusColours(TextBox tb)
        {
            tb.Background = FocusTextBoxBackgroundBrush;
            tb.BorderBrush = FocusTextBoxBorderBrush;
        }

        public void SetNotEnabledTextBoxFocusColours(TextBox tb)
        {
            tb.Background = FocusNotEnabledTextBoxBackgroundBrush;
            tb.BorderBrush = FocusNotEnabledTextBoxBorderBrush;
            tb.Foreground = NormalTextColourBrush;
        }

        #endregion

        internal void SetTextBoxFocusColours(PasswordBox passwordBox)
        {
            passwordBox.Background = FocusTextBoxBackgroundBrush;
            passwordBox.BorderBrush = FocusTextBoxBorderBrush;
        }
    }
}
