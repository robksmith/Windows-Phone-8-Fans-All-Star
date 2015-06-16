
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

namespace Zengo.WP8.FAS.Controls
{
    public partial class PopupAppyingUpdatesControl : UserControl
    {
        #region Constructors

        public PopupAppyingUpdatesControl()
        {
            InitializeComponent();
        }

        #endregion

        internal void SetProgress(int percent)
        {
            TextblockProgress.Text = percent.ToString() + "%";
        }
    }
}
