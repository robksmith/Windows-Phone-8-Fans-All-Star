
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
    public partial class BrowsePage : PhoneApplicationPage
    {
        #region Fields

        #endregion


        #region Constructors

        public BrowsePage()
        {
            InitializeComponent();

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
            if (parameters.ContainsKey("url"))
            {
                string url = parameters["url"];

                BrowserControl.IsScriptEnabled = true;
                BrowserControl.Loaded += BrowserControl_Loaded;
                BrowserControl.Navigating += BrowserControl_Navigating;
                BrowserControl.Navigated += BrowserControl_Navigated;
                BrowserControl.NavigationFailed += BrowserControl_NavigationFailed;
                BrowserControl.Navigate(new Uri(url));
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
            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "navigation failed" }, new TimeSpan(0, 0, 3)));
        }

        #endregion



        #region Event Handlers

        /// <summary>
        /// They have pressed the back button on the app bar
        /// </summary>
        void AppBarMenuItemGoBack_Click(object sender, EventArgs e)
        {
            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }

        /// <summary>
        /// They have pressed the back button
        /// </summary>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        #endregion
    }
}