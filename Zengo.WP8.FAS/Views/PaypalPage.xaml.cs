
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class PaypalPage : PhoneApplicationPage
    {
        #region Fields

        // simulate a purchase for testing
        bool simulateConfirmationPage = false;

        // has the confirmation message been found
        bool confirmationMessageFound = false;

        // The delay timer
        DispatcherTimer timer;

        // Delay between polling
        TimeSpan intervalDelay = new TimeSpan(0, 0, 5);

        // Poll the transactions api
        UserApi userApi = new UserApi();
        /*
                        <div>
                            <form method=""post"" action=""https://www.paypal.com/cgi-bin/webscr"" id=""paypal-form"">
                            <input type=""hidden"" value=""_s-xclick"" name=""cmd"">
                            <input type=""hidden""
                            value=""https://api.fas.moshenmedia.co.uk/ipn/uuid:{UserId}"" id=""paypal-ipn"" name=""notify_url"">
                            <input type=""hidden"" value=""{BidId}"" id=""paypal-bid""
                            name=""hosted_button_id"">
                            </form>
                        </div>
        */
        // Html to display in browser
        string html =
           @"
                <html >
    
                    <body style=""background-color:White;text-align:center;"">

                        <div id=""header"">

                            <div id=""content"" style=""background-color:#EEEEEE;margin:60px;padding:20px;"">
                                You are being redirected to the paypal website to complete your transaction.
                            </div>

                            <div id=""content2"" style=""background-color:#EEEEEE;margin:60px;padding:20px;"">
                                Please make sure you have internet access.
                            </div>

                        </div>

                        <div>
                            <form method=""post"" action=""https://www.paypal.com/cgi-bin/webscr"" id=""paypal-form"">
                            <input type=""hidden"" value=""_s-xclick"" name=""cmd"">
                            <input type=""hidden""
                            value=""https://api.fas.moshenmedia.co.uk/ipn/uuid:{UserId}"" id=""paypal-ipn"" name=""notify_url"">
                            <input type=""hidden"" value=""{BidId}"" id=""paypal-bid""
                            name=""hosted_button_id"">
                            </form>
                        </div>

                    </body>

                </html>

                <script type=""text/javascript"">
                    function myfunc () 
                    {
                        var frm = document.getElementById(""paypal-form"");
                        frm.submit();
                    }
                    window.onload = myfunc;
                </script>
                ";

        // Html to display in browser
        string htmlFailed =
           @"
                <html >
    
                    <body style=""background-color:White;text-align:center;"">

                        <div id=""header"">

                            <div id=""content"" style=""background-color:#EEEEEE;margin:60px;padding:20px;"">
                                Navigation to paypal failed - check you have internet access.
                            </div>

                            <div id=""content2"" style=""background-color:#EEEEEE;margin:60px;padding:20px;"">
                                Please go back and try again later.
                            </div>

                        </div>

                    </body>

                </html>
                ";
        #endregion


        #region Constructors

        public PaypalPage()
        {
            InitializeComponent();

            // Start timer which checks for known paypal confirmation text
            StartTimer();
            BuildApplicationBar();
            ApplicationBar.IsVisible = false;
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/back.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarGoBack };
            button.Click += AppBarMenuItemGoBack_Click;
            ApplicationBar.Buttons.Add(button);

        }
        #endregion


        #region Event Handlers

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Get the required bid id an initialise the browser
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("Bid"))
            {
                string bid = parameters["Bid"];

                string userId = App.ViewModel.DbViewModel.IsLoggedOn() ? App.ViewModel.DbViewModel.CurrentUser.UserId : "none";

                html = html.Replace("{UserId}", userId);
                html = html.Replace("{BidId}", bid);

                BrowserControl.IsScriptEnabled = true;
                BrowserControl.Loaded += BrowserControl_Loaded;
                BrowserControl.Navigating += BrowserControl_Navigating;
                BrowserControl.Navigated += BrowserControl_Navigated;
                BrowserControl.NavigationFailed += BrowserControl_NavigationFailed;
                BrowserControl.NavigateToString(html);
            }
        }

        void BrowserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new MessageControl() { Message = "Browser control loaded" }, new TimeSpan(0, 0, 1)));
        }

        void BrowserControl_Navigating(object sender, NavigatingEventArgs e)
        {
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new MessageControl() { Message = "navigating uri " + e.Uri }, new TimeSpan(0, 0, 1)));
        }

        void BrowserControl_Navigated(object sender, NavigationEventArgs e)
        {
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new MessageControl() { Message = "navigated " }, new TimeSpan(0, 0, 1)));
        }

        void BrowserControl_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
#if DEBUG
            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = AppResources.PayPalNavFailed }, new TimeSpan(0, 0, 1)));
#endif
            BrowserControl.NavigateToString(htmlFailed);
        }

        #endregion


        #region Timer and Api Events

        /// <summary>
        /// In here we need to do our polling check for new transactions / votes
        /// </summary>
        void timer_End_Tick(object sender, EventArgs e)
        {
            // Check the html displays a confirmation message - if so popup a message
            CheckForConfirmationMessage();
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// They have pressed the back button on the app bar
        /// </summary>
        void AppBarMenuItemGoBack_Click(object sender, EventArgs e)
        {
            // Stop the timer that checks for paypal text
            StopTimer();

            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }

        /// <summary>
        /// They have pressed the back button
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            // Stop the timer that checks for paypal text
            StopTimer();
        }


        #endregion


        #region Helpers
        
        /// <summary>
        /// Initialise the timer
        /// </summary>
        private void StartTimer()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            if (timer != null)
            {
                timer.Interval = intervalDelay;
                timer.Tick += new EventHandler(timer_End_Tick);
                timer.Start();
            }
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        private void StopTimer()
        {
            // stop the timer
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= new EventHandler(timer_End_Tick);
                timer = null;
            }
        }
        
        /// <summary>
        /// On the timer tick, we get the html - if it displays the know confirmation text, we display a message to the user
        /// </summary>
        private void CheckForConfirmationMessage()
        {
            try
            {
                // Get the htnml
                string h = BrowserControl.SaveToString().ToLower();

                // Check for known paypal text
                if (simulateConfirmationPage || h.Contains("confirmation") && h.Contains("transaction id"))
                {
                    if (!confirmationMessageFound)
                    {
                        // Stop the timer that calls this routine
                        StopTimer();

                        confirmationMessageFound = true;

                        // turn on the app bar so they can go back
                        ApplicationBar.IsVisible = true;

                        // We want to check transactions and votes up to 5 times each
                        //App.UserDataPollHelper.ResetTransactionAttempts(10);
                        //App.UserDataPollHelper.ResetVoteAttempts(10);

                        // Show the message
                        App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageTransactionCompleteControl(), new TimeSpan(0, 0, 6)));
                    }
                }
            }
            catch
            {
                // do nothing
            }
        }

        #endregion
    }
}