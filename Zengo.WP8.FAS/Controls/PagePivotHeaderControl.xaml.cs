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
    public partial class PagePivotHeaderControl : UserControl
    {
        public string PageName { get { return TextboxPageName.Text; } set
        {
            TextboxPageName.Text = value;
        } }

        public PagePivotHeaderControl()
        {
            InitializeComponent();

            EnableProgresBar(false);
        }

        internal void EnableProgresBar(bool enabled)
        {
            //progressBar.Visibility = enabled == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
    }
}
