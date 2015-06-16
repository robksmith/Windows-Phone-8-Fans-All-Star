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
    public partial class HelpPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton faqButton;

        public HelpPage()
        {
            InitializeComponent();

            PageHeaderControl.PageName = AppResources.HelpPage;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;

            BuildApplicationBar();
        }


        #region Helpers

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            faqButton = new ApplicationBarIconButton(new Uri("/Images/AppBar/faq.png", UriKind.RelativeOrAbsolute)) { Text = "FAQ" };
            faqButton.Click += faqButton_Click;
            ApplicationBar.Buttons.Add(faqButton);
        }

        #endregion



        #region Event Handlers

        void faqButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FAQPage.xaml", UriKind.RelativeOrAbsolute));
        }

        #endregion

    }
}