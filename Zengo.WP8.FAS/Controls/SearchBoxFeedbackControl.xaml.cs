using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Zengo.WP8.FAS.Controls
{
    public partial class SearchBoxFeedbackControl : UserControl
    {

        #region Events

        public event EventHandler<EventArgs> CancelSearch;

        #endregion


        public SearchBoxFeedbackControl()
        {
            InitializeComponent();
        }



        /// <summary>
        /// Clear the search
        /// </summary>
        private void HyperlinkClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (CancelSearch != null)
            {
                CancelSearch(this, new EventArgs());
            }

        }


        internal void Enable(bool enable, int results, string searchText)
        {
            if (enable)
            {
                RowFeedback.Visibility = System.Windows.Visibility.Visible;
                TextBlockFoundResults.Text = string.Format("found {0} results for", results);
                TextBlockSearchTerm.Text = searchText;
            }
            else
            {
                RowFeedback.Visibility = System.Windows.Visibility.Collapsed;
                TextBlockFoundResults.Text = string.Empty;
                TextBlockSearchTerm.Text = string.Empty;
            }
        }
    }
}
