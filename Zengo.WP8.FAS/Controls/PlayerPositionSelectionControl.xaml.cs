
#region Usings

using Zengo.WP8.FAS.WebApi.Responses;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Navigation;
using System.Collections.Generic;
using System;
using System.Windows.Media.Animation;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Helpers;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PlayerPositionSelectionControl : UserControl
    {
        #region Fields

        #endregion


        #region Events

        public event EventHandler<PlayerSelectionIconControl.PlayerTapEventArgs> PlayerTapped;

        #endregion


        #region Constructors

        public PlayerPositionSelectionControl()
        {
            InitializeComponent();

            // Set up the event handlers for the players icons
            foreach (UserControl c in Canvas2.Children)
            {
                if (c is PlayerSelectionIconControl)
                {
                    PlayerSelectionIconControl icon = c as PlayerSelectionIconControl;
                    icon.PlayerTapped += Player_Tapped;

                    icon.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
                    icon.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
                }
            }
        }

        #endregion


        #region Event Handlers

        void Player_Tapped(object sender, PlayerSelectionIconControl.PlayerTapEventArgs e)
        {
            if (PlayerTapped != null)
            {
                PlayerTapped(this, e);
            }
        }

        #endregion


        #region Helpers

        internal void HighlightPosition(PositionRecord positionRecord)
        {
            foreach (UserControl c in Canvas2.Children)
            {
                if (c is PlayerSelectionIconControl)
                {
                    PlayerSelectionIconControl icon = c as PlayerSelectionIconControl;

                    icon.PlayerOpacity = 0.5;

                    if (icon.PositionId == positionRecord.PositionId)
                    {
                        icon.PlayerOpacity = 1.0;
                    }
                }
            }
        }

        internal void Reload()
        {
            foreach (UserControl c in Canvas2.Children)
            {
                if (c is PlayerSelectionIconControl)
                {
                    PlayerSelectionIconControl icon = c as PlayerSelectionIconControl;
                    icon.Reload();
                }
            }
        }

        /// <summary>
        /// Given a zone, set up the control to display positions from that zone
        /// </summary>
        internal void Configure(PlayerRecord player, PositionRecord position)
        {
            ZoneRecord zoneRecord = player.Zone;

            foreach (UserControl c in Canvas2.Children)
            {
                if (c is PlayerSelectionIconControl)
                {
                    PlayerSelectionIconControl icon = c as PlayerSelectionIconControl;

                    icon.PlayerOpacity = 0.5;
                    icon.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            if (zoneRecord.Group == PitchConstants.ZoneKeeper)
            {
                PlayerGoalkeper.Visibility = System.Windows.Visibility.Visible;
		    	PlayerSub.Visibility = System.Windows.Visibility.Visible;

                PlayerGoalkeper.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyKeeper, player.IsEurope);
                PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubKeeper, player.IsEurope);
            }
            if (zoneRecord.Group == PitchConstants.ZoneDefence)
            {
                if (true)
                {
                    if (zoneRecord.ZoneId == PitchConstants.ZoneRightBackUuid)
                    {
                        Player1.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightBack, player.IsEurope);
                        //PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubDefence, player.IsEurope);

                        Player1.Visibility = System.Windows.Visibility.Visible;
                        //PlayerSub.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (zoneRecord.ZoneId == PitchConstants.ZoneLeftBackUuid)
                    {
                        Player4.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftBack, player.IsEurope);
                        //PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubDefence, player.IsEurope);

                        Player4.Visibility = System.Windows.Visibility.Visible;
                        //PlayerSub.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        Player2.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightCenterBack, player.IsEurope);
                        Player3.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftCenterBack, player.IsEurope);
                        PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubDefence, player.IsEurope);

                        Player2.Visibility = System.Windows.Visibility.Visible;
                        Player3.Visibility = System.Windows.Visibility.Visible;
                        PlayerSub.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    Player1.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightBack, player.IsEurope);
                    Player2.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightCenterBack, player.IsEurope);
                    Player3.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftCenterBack, player.IsEurope);
                    Player4.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftBack, player.IsEurope);
                    PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubDefence, player.IsEurope);

                    Player1.Visibility = System.Windows.Visibility.Visible;
                    Player2.Visibility = System.Windows.Visibility.Visible;
                    Player3.Visibility = System.Windows.Visibility.Visible;
                    Player4.Visibility = System.Windows.Visibility.Visible;
                    PlayerSub.Visibility = System.Windows.Visibility.Visible;
                }
            }
            if (zoneRecord.Group == PitchConstants.ZoneMidfield)
            {
                Player1.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightMidfield, player.IsEurope);
                Player2.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightCenterMidfield, player.IsEurope);
                Player3.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftCenterMidfield, player.IsEurope);
                Player4.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftMidfield, player.IsEurope);
                PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubMidfield, player.IsEurope);

                Player1.Visibility = System.Windows.Visibility.Visible;
                Player2.Visibility = System.Windows.Visibility.Visible;
                Player3.Visibility = System.Windows.Visibility.Visible;
                Player4.Visibility = System.Windows.Visibility.Visible;
                PlayerSub.Visibility = System.Windows.Visibility.Visible;
            }
            if (zoneRecord.Group == PitchConstants.ZoneForward)
            {
                Player2.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyRightForward, player.IsEurope);
                Player3.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeyLeftForward, player.IsEurope);
                PlayerSub.PositionId = PitchConstants.PositionIdFromKey(PitchConstants.PositionKeySubForward, player.IsEurope);

                Player2.Visibility = System.Windows.Visibility.Visible;
                Player3.Visibility = System.Windows.Visibility.Visible;
                PlayerSub.Visibility = System.Windows.Visibility.Visible;
            }

            Reload();
        }

        #endregion


    }
}
