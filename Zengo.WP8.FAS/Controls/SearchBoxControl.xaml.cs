
#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion


namespace Zengo.WP8.FAS.Controls
{
    public partial class SearchBoxControl : UserControl
    {
        public class DoSearchEventArgs : EventArgs
        {
            public string searchTerm { get; set; }
        }


        #region Events

        public event EventHandler<DoSearchEventArgs> DoSearch;
        public event EventHandler<EventArgs> SearchCancelled;

        #endregion


        #region Constructors

        public SearchBoxControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        public string Text 
        { 
            get
            {
                return TextBoxSearchFind.Text;
            }
            set 
            {
                TextBoxSearchFind.Text = value;
            } 
        }

        #endregion


        internal void SelectAll()
        {
            TextBoxSearchFind.Focus();
        }


        internal bool IsSearchEnabled()
        {
            return BorderSearch.Visibility == System.Windows.Visibility.Visible ? true : false;
        }

        internal void EnableSearch( bool enable)
        {
            if (enable)
            {
                BorderSearch.Visibility = System.Windows.Visibility.Visible;
                TextBoxSearchFind.Focus();
            }
            else
            {
                BorderSearch.Visibility = System.Windows.Visibility.Collapsed;
            }
        }



        /// <summary>
        /// The user clicks on the search textbox (giving it focus)
        /// </summary>
        private void TextBoxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            App.AppConstants.SetTextBoxFocusColours(sender as TextBox);

            SelectAll();
        }

        /// <summary>
        /// If return key is pressed, remove the search textbox and do the search
        /// </summary>
        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Text.Length >= 2)
                {
                    if (DoSearch != null)
                    {
                        DoSearch(this, new DoSearchEventArgs() { searchTerm = TextBoxSearchFind.Text });
                    }
                }
            }
        }

        /// <summary>
        /// Lost focus - let the container know so they can do what they need to do
        /// </summary>
        private void TextBoxSearch_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            EnableSearch(false);

            if (SearchCancelled != null)
            {
                SearchCancelled(this, new EventArgs());
            }
        }

    }
}
