
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls.ListItems;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class MyCountryPage : PhoneApplicationPage
    {
        #region Fields

        // ascending or descending
        ListSortDirection sortDirection;

        // the search expression
        string searchTerm;

        // a couple of constant 
        const int searchRow = 1;
        const double searchHeight = 70;

        const int feedbackRow = 3;
        const double feedbackHeight = 70;

        #endregion


        #region Constructors

        public MyCountryPage()
        {
            InitializeComponent();

            // Default search terms
            sortDirection = ListSortDirection.Ascending;
            searchTerm = string.Empty;

            // Do the default search
            PopulateList();

            // Subscribe to the selection changed event
            CountriesList.SelectionChanged += CountriesList_SelectionChanged;

            // We want to know when the user invokes a search
            SearchBox.DoSearch += SearchBox_DoSearch;

            // We want to know when the search feedback has had the clear search button pressed
            SearchBoxFeedback.CancelSearch += SearchBoxResults_CancelSearch;

            PageHeaderControl.PageName = AppResources.MyCountryPage;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;

            BuildApplicationBar();

            // make sure the app bar menu has the correct text
            UpdateAppBarMenu();
        }

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var button = new ApplicationBarIconButton(new Uri("/Images/AppBar/search.png", UriKind.RelativeOrAbsolute)) { Text = AppResources.AppBarSearch };
            button.Click += AppBarButtonSearch_Click;
            ApplicationBar.Buttons.Add(button);

            var item = new ApplicationBarMenuItem { Text = AppResources.AppBarSortAscending };
            item.Click += AppBarMenuItemSortAscending_Click;
            ApplicationBar.MenuItems.Add(item);
        }

        #endregion


        #region Page Handlers

        #endregion


        #region Event Handlers

        /// <summary>
        /// User has made a selection from the long list
        /// </summary>
        void CountriesList_SelectionChanged(object sender, Controls.LongListCountriesControl.CountrySelectionChangedEventArgs e)
        {
            // Record the country selected
            App.AppConstants.MyCountry = e.Country;

            // Go back to the register page
            NavigationService.GoBack();
        }


        /// <summary>
        /// User has invoked a search
        /// </summary>
        void SearchBox_DoSearch(object sender, Controls.SearchBoxControl.DoSearchEventArgs e)
        {
            // Return focus back to screen - get rid of the keyboard
            this.Focus();

            // grab the search term
            searchTerm = e.searchTerm;

            // Do the search / sort
            PopulateList();

            // Disable search box - do this to close the row
            EnableSearch(false);

            // Turn on the search info box and fill in teh nmber of results
            EnableInfo(true, App.ViewModel.DbViewModel.CountriesCount(true, searchTerm), searchTerm);
        }


        /// <summary>
        /// Clear the search
        /// </summary>
        void SearchBoxResults_CancelSearch(object sender, EventArgs e)
        {
            // Search term is empty
            searchTerm = string.Empty;

            // Populate the list
            PopulateList();

            // Disable search box
            EnableSearch(false);

            // Disable info box
            EnableInfo(false, 0, string.Empty);
        }

        #endregion


        #region App Bar Events

        /// <summary>
        /// Alternate between sort ascending and descending
        /// </summary>
        void AppBarMenuItemSortAscending_Click(object sender, EventArgs e)
        {
            //Toggle the search direction
            sortDirection = sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            // make sure the app bar menu has the correct text
            UpdateAppBarMenu();

            // Populate the list
            PopulateList();
        }


        /// <summary>
        /// The search button on the app bar has been pressed
        /// </summary>
        private void AppBarButtonSearch_Click(object sender, System.EventArgs e)
        {
            EnableSearch(!SearchBox.IsSearchEnabled());
        }

        #endregion


        #region Helpers

        internal void EnableSearch(bool enable)
        {
            if (enable)
            {
                LayoutRoot.RowDefinitions[searchRow].Height = new GridLength(searchHeight);
            }
            else
            {
                LayoutRoot.RowDefinitions[searchRow].Height = new GridLength(0);
                this.Focus();
            }

            SearchBox.EnableSearch(enable);
        }

        private void EnableInfo(bool enable, int results, string searchText)
        {
            if (enable)
            {
                LayoutRoot.RowDefinitions[feedbackRow].Height = new GridLength(feedbackHeight);
            }
            else
            {
                LayoutRoot.RowDefinitions[feedbackRow].Height = new GridLength(0);
            }

            SearchBoxFeedback.Enable(enable, results, searchTerm);
        }

        private void PopulateList()
        {
            CountriesList.countriesLongList.ItemsSource = (System.Collections.IList)App.ViewModel.DbViewModel.CountriesKeyList(false, sortDirection, searchTerm);
        }

        /// <summary>
        /// Update the text for the app bar menu depending on sort ascending / descending
        /// </summary>
        private void UpdateAppBarMenu()
        {
            ApplicationBarMenuItem m = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            m.Text = sortDirection == ListSortDirection.Ascending ? AppResources.AppBarSortDescending : AppResources.AppBarSortAscending;
        }

        #endregion
    }
}