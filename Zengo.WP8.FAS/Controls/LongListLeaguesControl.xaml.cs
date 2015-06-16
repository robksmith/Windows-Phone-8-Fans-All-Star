using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Models;

namespace Zengo.WP8.FAS.Controls
{
    public partial class LongListLeaguesControl : UserControl
    {
        public class LeagueSelectionChangedEventArgs : EventArgs
        {
            public LeagueAnswer League { get; set; }
        }

        #region Events

        public event EventHandler<LeagueSelectionChangedEventArgs> SelectionChanged;
 
        #endregion

        public LongListLeaguesControl()
        {
            InitializeComponent();

            LeagueLongList.SelectionChanged += LeagueLongListOnSelectionChanged;
        }

        private void LeagueLongListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new LeagueSelectionChangedEventArgs { League = (LeagueAnswer)LeagueLongList.SelectedItem });
            }
        }
    }
}
