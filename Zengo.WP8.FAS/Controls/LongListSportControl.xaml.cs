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
    public partial class LongListSportControl : UserControl
    {
       public class SportSelectionChangedEventArgs : EventArgs
        {
            public SportAnswer Sport { get; set; }
        }

        #region Events

       public event EventHandler<SportSelectionChangedEventArgs> SelectionChanged;
 
        #endregion

        public LongListSportControl()
        {
            InitializeComponent();

            SportLongList.SelectionChanged += StadiumLongListOnSelectionChanged;
        }

        private void StadiumLongListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new SportSelectionChangedEventArgs { Sport = (SportAnswer)SportLongList.SelectedItem });
            }
        }
    }
}
