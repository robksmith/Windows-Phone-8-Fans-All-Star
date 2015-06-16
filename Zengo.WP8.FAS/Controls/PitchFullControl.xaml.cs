
#region Usings

using Zengo.WP8.FAS.WebApi.Responses;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Navigation;
using System.Collections.Generic;
using System;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PitchFullControl : UserControl
    {
        #region Fields

        #endregion


        #region Events

        public event EventHandler<PlayerIconTapEventArgs> PlayerTapped;

        #endregion


        public PitchFullControl()
        {
            InitializeComponent();

            // Set up the event handlers for the players icons
            foreach (UserControl c in CanvasFullPitch.Children)
            {
                if (c is PlayerSmallIconControl)
                {
                    PlayerSmallIconControl icon = c as PlayerSmallIconControl;
                    icon.PlayerTapped += Player_Tapped;

                    icon.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
                    icon.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
                }
            }
        }


        #region Event Handlers

        void Player_Tapped(object sender, PlayerIconTapEventArgs e)
        {
            if (PlayerTapped != null)
            {
                PlayerTapped(this, e);
            }
        }

        #endregion


        #region Helpers

        internal void Reload(PitchRecord pitch)
        {
            foreach (UserControl c in CanvasFullPitch.Children)
            {
                if (c is PlayerSmallIconControl)
                {
                    PlayerSmallIconControl icon = c as PlayerSmallIconControl;
                    icon.Reload(pitch);
                }
            }
        }

        #endregion

    }
}
