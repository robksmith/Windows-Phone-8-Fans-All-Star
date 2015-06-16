using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Resources;

namespace Zengo.WP8.FAS
{
    public partial class FAQPage : PhoneApplicationPage
    {
        public FAQPage()
        {
            InitializeComponent();

            PageHeaderControl.PageName = AppResources.FAQPageTitle;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;
        }
    }
}