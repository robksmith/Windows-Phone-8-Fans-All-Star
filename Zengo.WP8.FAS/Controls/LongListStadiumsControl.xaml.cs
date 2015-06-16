using System;
using System.Windows.Controls;
using Zengo.WP8.FAS.Models;

namespace Zengo.WP8.FAS.Controls
{
    public partial class LongListStadiumsControl : UserControl
    {
        public class StadiumSelectionChangedEventArgs : EventArgs
        {
            public StadiumAnswer Stadium { get; set; }
        }

        #region Events

        public event EventHandler<StadiumSelectionChangedEventArgs> SelectionChanged;
 
        #endregion

       public LongListStadiumsControl()
        {
            InitializeComponent();

            StadiumLongList.SelectionChanged += StadiumLongListOnSelectionChanged;
        }

        private void StadiumLongListOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new StadiumSelectionChangedEventArgs { Stadium = (StadiumAnswer)StadiumLongList.SelectedItem });
            }
        }
    }
}
