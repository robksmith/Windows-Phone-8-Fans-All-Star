
#region Usings

using System.Globalization;
using System.IO.IsolatedStorage;
using System.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Helpers;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.WepApi.Api;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class App : Application
    {
        #region Fields

        private bool phoneApplicationInitialized = false;

        private static MainViewModel viewModel = null;

        private static JsonDataPollHelper jsonDataPollHelper = null;

        private static UserDataPollHelper userDataPollHelper = null;

        private static PopupApplyingUpdatesHelper popupApplyingUpdatesHelper = null;

        private static PopupHelper popupHelper = null;

        private static AppConstants appConstants = null;
        
        private DateTime appStartTime;

        #endregion


        #region Properties

        public static LanguageSettingsViewModel LanguageViewModel { get; private set; }

        public static FreeEntryViewModel FreeEntryViewModel { get; private set; }

        /// <summary>
        /// A static ViewModel used by the views to bind against.
        /// </summary>
        public static MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                    viewModel = new MainViewModel();

                return viewModel;
            }
        }

        public static JsonDataPollHelper JsonDataPollHelper
        {
            get
            {
                if (jsonDataPollHelper == null)
                    jsonDataPollHelper = new JsonDataPollHelper();

                return jsonDataPollHelper;
            }
        }

        public static UserDataPollHelper UserDataPollHelper
        {
            get
            {
                if (userDataPollHelper == null)
                    userDataPollHelper = new UserDataPollHelper();

                return userDataPollHelper;
            }
        }


        public static PopupApplyingUpdatesHelper PopupApplyingUpdatesHelper
        {
            get
            {
                if (popupApplyingUpdatesHelper == null)
                    popupApplyingUpdatesHelper = new PopupApplyingUpdatesHelper();

                return popupApplyingUpdatesHelper;
            }
        }


        public static PopupHelper PopupHelper
        {
            get
            {
                if (popupHelper == null)
                    popupHelper = new PopupHelper();

                return popupHelper;
            }
        }

        public static AppConstants AppConstants
        {
            get
            {
                if (appConstants == null)
                    appConstants = new AppConstants();

                return appConstants;
            }
        }

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // record the app start time
            appStartTime = DateTime.Now;

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            IsolatedStorageSettings isolatedStorage = IsolatedStorageSettings.ApplicationSettings;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Do what we need to do to initialise a new database
            DatabaseHelper.CreateOrCopyTheSeedDatabase();

            if (!isolatedStorage.Contains("language"))
            {
                isolatedStorage.Add("language", "en");
                isolatedStorage.Save();
            }
            LanguageViewModel = (LanguageSettingsViewModel)Resources["LanguageDataSource"];
            LanguageViewModel.SetLanguageFromCurrentLocate();
            

            FreeEntryViewModel = (FreeEntryViewModel) Resources["FreeEntryViewModel"];
            FreeEntryViewModel.ReadBatch();
            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            RootFrame.Navigating += RootFrame_Navigating;
            RootFrame.Navigated += RootFrame_Navigated;
            RootFrame.BackKeyPress += RootFrame_BackKeyPress;

            LoadingHelper.Instance.Initialize(RootFrame);
        }

        #endregion


        #region Event Handlers

        void RootFrame_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        /// <summary>
        /// We want to cature a navigating event - if its to the dummy landing page, then check whether we are logged in and send to the relevant page
        /// </summary>
        void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // Only care about our landing page 
            if (e.Uri.ToString().Contains("/LandingPage.xaml"))
            {
                bool loggedIn = App.ViewModel.DbViewModel.IsLoggedOn();

                // Work out whether we need to hold the splash screen - otherwise on fast devices it just flashes on and off the screen
                TimeSpan timeTakenToGetHere = DateTime.Now - appStartTime;
                TimeSpan timeToSleepFor = TimeSpan.FromSeconds(App.AppConstants.SecondsToHoldSplashScreen) - timeTakenToGetHere;

                if (timeToSleepFor > TimeSpan.FromSeconds(0))
                {
                    System.Threading.Thread.Sleep(timeToSleepFor);
                }

                // Cancel current navigation and schedule the real navigation for the next tick 
                // (we can't navigate immediately as that will fail; no overlapping navigations are allowed) 
                e.Cancel = true;
                RootFrame.Dispatcher.BeginInvoke(delegate
                {
                    if (loggedIn)
                    {
                        RootFrame.Navigate(new Uri("/Views/MainPage.xaml?method=cancel", UriKind.Relative));
                    }
                    else
                    {
                        //RootFrame.Navigate(new Uri("/WelcomePage.xaml?method=cancel", UriKind.Relative));
                        RootFrame.Navigate(new Uri("/Views/MainPage.xaml?method=cancel", UriKind.Relative));
                    }
                });
            }
        }

        /// <summary>
        /// On each page we want the system tray to be the same colour - even for the light theme
        /// </summary>
        void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Microsoft.Phone.Shell.SystemTray.BackgroundColor = Colors.Black;
            Microsoft.Phone.Shell.SystemTray.ForegroundColor = Colors.White;
        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //App.ViewModel.Profile = IsolatedStorage.GetProfile(IsolatedStorage.ProfileFilename);

            //StateUtilities.IsLaunching = true;

            //App.UpdateHelper.FetchUpdateBatch();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //StateUtilities.IsLaunching = false;

            //App.ViewModel.Profile = IsolatedStorage.GetProfile(IsolatedStorage.ProfileFilename);

            //App.UpdateHelper.FetchUpdateBatch();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            //IsolatedStorage.SaveProfile(App.ViewModel.Profile, IsolatedStorage.ProfileFilename);
            //DatabaseHelper.FasDataContext.Dispose();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Ensure that required application state is persisted here.
            //IsolatedStorage.SaveProfile(App.ViewModel.Profile, IsolatedStorage.ProfileFilename);
            DatabaseHelper.FasDataContext.Dispose();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #endregion


        #region Phone application initialization

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion


        #region App Bar Events

        private void Button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 1 works!");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button 2 works!");
        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Menu item 1 works!");
        }

        private void MenuItem2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Menu item 2 works!");
        }

        #endregion

    }
}