
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.Models;
using System.Globalization;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class UpdateCheckPage : PhoneApplicationPage
    {
        #region Events

        bool showAll;

        #endregion


        #region Properties

        public UpdateCheckPage()
        {
            InitializeComponent();

            //App.ViewModel.DbViewModel.LoadUpdatesFromDatabase();

            // Populate the long list
            PopulateList();

            // Subscribe to the tap event
            UpdatesList.Tap += UpdatesList_Tap;

            // Create an app bar
            ApplicationBar = new ApplicationBar();

            PageHeaderControl.PageName = AppResources.UpdatesTitle;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;
            // Enable the app menu
            CreateApplicationBar();
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// An entry has been tapped
        /// </summary>
        void UpdatesList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UpdateCheckRecord uc = (UpdateCheckRecord)UpdatesList.SelectedItem;

            if (uc != null)
            {
                string para = string.Empty;
                foreach (ApiUpdateRecord update in uc.Updates)
                {
                    string line = string.Format("{0}: {1} records {2}", update.ApiName, update.RecordCount.ToString(), Environment.NewLine);
                    para += line;
                }

                MessageBox.Show(para, string.Format(AppResources.APIUpdates, uc.Updates.Count.ToString()), MessageBoxButton.OK);
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Create our application bar 
        /// </summary>
        void CreateApplicationBar()
        {
            // clear the app bar
            ApplicationBar.MenuItems.Clear();
            ApplicationBar.Buttons.Clear();

            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;

            ApplicationBarMenuItem selectMenuItem = new ApplicationBarMenuItem();
            selectMenuItem.Text = showAll == true ? AppResources.ShowApisNeeded : AppResources.ShowAllApis;
            ApplicationBar.MenuItems.Add(selectMenuItem);
            selectMenuItem.Click += selectMenuItem_Click;
        }

        /// <summary>
        /// The menu item has been selected
        /// </summary>
        void selectMenuItem_Click(object sender, EventArgs e)
        {
            // toggle
            showAll ^= true;

            // repopulate the list
            PopulateList();

            // set up the app bar with proper text
            CreateApplicationBar();
        }

        /// <summary>
        /// Get data and fill in the list
        /// </summary>
        private void PopulateList()
        {
            if (showAll)
            {
                UpdatesList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.UpdatesAll();
            }
            else
            {
                UpdatesList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.UpdatesWhereRequired();
            }

            TextLastSuccessfulUpdate.Text = string.Format(AppResources.LastVoteUpdate, App.ViewModel.DbViewModel.LastSuccessfullUpdate().DateTime.ToString(CultureInfo.InvariantCulture));
            //TextLastSuccessfulUpdate.Text = "Last successfull update " + App.ViewModel.DbViewModel.LastSuccessfullUpdate().DateTime.ToString();
        }

        #endregion


        #region Animation

        private void UpdateCheckPage_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };
        }

        private void UpdateCheckPage_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

        #endregion
    }


}