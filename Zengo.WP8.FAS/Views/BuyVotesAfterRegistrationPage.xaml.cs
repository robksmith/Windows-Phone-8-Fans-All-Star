
#region Usings

using Microsoft.Phone.Controls;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class BuyVotesAfterRegistrationPage : PhoneApplicationPage
    {
        #region Constructors

        public BuyVotesAfterRegistrationPage()
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

                if (package.Price == 0 || package.PackageId == PackageRecord.FreeId)
                {
                    // If they just want to use their free vote then send them back
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
                else
                {
                    // Otherwise send them to the paypal browser
                    this.NavigationService.Navigate(new Uri("/Views/PaypalPage.xaml?Bid=" + package.Bid, UriKind.Relative));
                }
            }
        }

        #endregion
    }
}