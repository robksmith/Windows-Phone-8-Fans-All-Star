
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

#endregion

namespace Zengo.WP8.FAS
{
    public partial class WelcomePage : PhoneApplicationPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        //private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/MainPage.xaml?redirect=login", UriKind.Relative));
        //}

        //private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/RegisterPage.xaml", UriKind.Relative));
        //}

        //private void ButtonResetPassword_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/RequestPasswordResetPinPage.xaml", UriKind.Relative));
        //}

        private void HyperlinkButtonGetStarted_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml?from=welcome", UriKind.Relative));
        }

        //private void HyperlinkRequestPin_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/RequestPasswordResetPinPage.xaml", UriKind.Relative));
        //}

        //private void HyperlinkIHavePin_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //}
    }
}