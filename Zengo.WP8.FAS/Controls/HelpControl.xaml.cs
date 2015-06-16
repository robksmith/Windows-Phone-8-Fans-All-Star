#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class HelpControl : UserControl
    {
        #region Constructors

        public HelpControl()
        {
            InitializeComponent();
        }

        #endregion

        //private void FAQButton_Click(object sender, RoutedEventArgs e)
        //{
        //    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/FAQPage.xaml", UriKind.RelativeOrAbsolute));
        //}
    }
}
