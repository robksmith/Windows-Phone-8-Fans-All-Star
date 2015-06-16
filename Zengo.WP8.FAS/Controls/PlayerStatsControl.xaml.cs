
#region Usings

using Zengo.WP8.FAS.Models;
using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PlayerStatsControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> BackPressed;

        #endregion


        #region Constructors

        public PlayerStatsControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Event Handlers

        private void HyperlinkButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (BackPressed != null)
            {
                BackPressed(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
