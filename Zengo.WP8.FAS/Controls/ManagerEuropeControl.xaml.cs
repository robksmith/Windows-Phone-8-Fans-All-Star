
#region Usings

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class ManagerEuropeControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> ClosePressed;

        #endregion


        #region Properties

        public ManagerEuropeControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Event Handlers
        
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (ClosePressed != null)
            {
                ClosePressed(this, EventArgs.Empty);
            }
        }

        #endregion

    }
}
