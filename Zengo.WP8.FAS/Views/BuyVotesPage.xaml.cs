
#region Usings

using Microsoft.Phone.Controls;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class BuyVotesPage : PhoneApplicationPage
    {
        #region Constructors

        public BuyVotesPage()
        {
            InitializeComponent();

            var packages = App.ViewModel.DbViewModel.PackagesList();

            PackageRecord free = new PackageRecord() { PackageId = PackageRecord.FreeId, Name = "Free Entry" };

            packages.Add(free);

            packagesList.ItemsSource = packages;

            PageHeaderControl.PageTitle = AppResources.ProductTitle;
            PageHeaderControl.PageName = AppResources.BuyVotesPage;
        }

        #endregion


        #region Page Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // If required, remove an entry from the back stack - this is used if we get here from the vote page
            if (NavigationContext.QueryString.ContainsKey("removeBackStack"))
            {
                string removeBackStack = NavigationContext.QueryString["removeBackStack"];
                if (removeBackStack == "yes")
                {
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.RemoveBackEntry();
                    }
                    // we need to remove this in case they press back from the browser page - we dont want this to run again
                    NavigationContext.QueryString.Remove("removeBackStack");
                }
            }

            //FreeEntryButton.IsEnabled = App.ViewModel.DbViewModel.CanEnableFreeQuestion();
        }

        #endregion


        #region Event Handlers

        private void ButtonBuy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Make sure they are logged on
            if (!App.ViewModel.DbViewModel.IsLoggedOn())
            {
                MessageBox.Show(AppResources.PurchaseLoginWarning);
                return;
            }

            // Send them to the paypal browser page
            if (sender is Button)
            {
                Button button = sender as Button;

                PackageRecord package = button.DataContext as PackageRecord;


                if (package.PackageId == PackageRecord.FreeId)
                {
                    if (App.ViewModel.DbViewModel.CanEnableFreeQuestion())
                    {
                        NavigationService.Navigate(new Uri("/Views/FreeEntryPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        //MessageBox.Show("You have already claimed your free votes", "Already claimed", MessageBoxButton.OK);
                        MessageBox.Show("To be eligible for free votes you must have already submitted at least one full team and you must not have not already claimed your free votes", "Not available", MessageBoxButton.OK);
                    }
                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/Views/PaypalPage.xaml?Bid=" + package.Bid, UriKind.Relative));
                }
            }
        }

        #endregion

        //private void FreeEntry_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/Views/FreeEntryPage.xaml", UriKind.Relative));
        //}
    }
}