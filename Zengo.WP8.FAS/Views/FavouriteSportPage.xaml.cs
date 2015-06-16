using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Resources;

namespace Zengo.WP8.FAS
{
    public partial class FavouriteSportPage : PhoneApplicationPage
    {
        #region Fields

        ListSortDirection sortDirection;

        private string searchTerm;

        private const int SearchRow = 1;
        private const double SearchHeight = 70;

        private const int FeedbackRow = 3;
        private const double FeedbackHeight = 70;

        #endregion

        #region Constructor

        public FavouriteSportPage()
        {
            InitializeComponent();

            sortDirection = ListSortDirection.Ascending;
            searchTerm = string.Empty;

            PopulateList();

            SportList.SelectionChanged += SportListOnSelectionChanged;

            SearchBox.DoSearch += SearchBox_DoSearch;

            SearchBoxFeedback.CancelSearch += SearchBoxResults_CancelSearch;

            PageHeaderControl.PageTitle = AppResources.FavouriteSportPage;
            PageHeaderControl.PageName = AppResources.ProductTitle;

            BuildApplicationBar();

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

        #region Event Handlers

        private void SportListOnSelectionChanged(object sender, LongListSportControl.SportSelectionChangedEventArgs e)
        {
            App.AppConstants.FavSport = e.Sport;

            NavigationService.GoBack();
        }

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

            EnableInfo(true, App.ViewModel.DbViewModel.PlayersCount(searchTerm));
        }

        void SearchBoxResults_CancelSearch(object sender, EventArgs e)
        {
            // Search term is empty
            searchTerm = string.Empty;

            // Populate the list
            PopulateList();

            // Disable search box
            EnableSearch(false);

            // Disable info box
            EnableInfo(false, 0);
        }

        #endregion

        #region AppBarEvents

        void AppBarMenuItemSortAscending_Click(object sender, EventArgs e)
        {
            //Toggle the search direction
            sortDirection = sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            // make sure the app bar menu has the correct text
            UpdateAppBarMenu();

            // Populate the list
            PopulateList();
        }

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
                LayoutRoot.RowDefinitions[SearchRow].Height = new GridLength(SearchHeight);
            }
            else
            {
                LayoutRoot.RowDefinitions[SearchRow].Height = new GridLength(0);
                Focus();
            }

            SearchBox.EnableSearch(enable);
        }

        private void EnableInfo(bool enable, int results)
        {
            LayoutRoot.RowDefinitions[FeedbackRow].Height = enable ? new GridLength(FeedbackHeight) : new GridLength(0);

            SearchBoxFeedback.Enable(enable, results, searchTerm);
        }

        private void PopulateList()
        {
            var list = App.FreeEntryViewModel.Sports(sortDirection, searchTerm);
            SportList.SportLongList.ItemsSource = (System.Collections.IList)list;
        }

        private void UpdateAppBarMenu()
        {
            var m = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            m.Text = sortDirection == ListSortDirection.Ascending ? AppResources.AppBarSortDescending : AppResources.AppBarSortAscending;
        }

        #endregion
    }
}