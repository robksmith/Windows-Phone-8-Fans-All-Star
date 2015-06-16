
#region Usings

using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PitchEuropeControl : UserControl
    {
        #region Fields

        #endregion


        #region Events

        public event EventHandler<PlayerIconTapEventArgs> PlayerTapped;
        //public event EventHandler<ManagerIconTapEventArgs> ManagerTapped;

        #endregion


        #region Constructors

        public PitchEuropeControl()
        {
            InitializeComponent();

            //EuropeManagers.FlyoutOpened += EuropeManagers_FlyoutOpened;
            //EuropeManagers.FlyoutClosed += EuropeManagers_FlyoutClosed;

            FlyoutSubstitutes.FlyoutOpened += EuropeSubstitutes_FlyoutOpened;
            FlyoutSubstitutes.FlyoutClosed += EuropeSubstitutes_FlyoutClosed;

            // Set up the event handlers for the players icons
            foreach (UserControl c in CanvasEurope.Children)
            {
                if (c is PlayerIconControl)
                {
                    PlayerIconControl icon = c as PlayerIconControl;
                    icon.PlayerTapped += Player_Tapped;

                    icon.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
                    icon.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
                }
            }

            FlyoutSubstitutes.PlayerTapped += Player_Tapped;
            //EuropeManagers.ManagerTapped += EuropeManagers_ManagerTapped;
        }

        #endregion


        #region Event Handlers

        void Player_Tapped(object sender, PlayerIconTapEventArgs e)
        {
            if (PlayerTapped != null)
            {
                PlayerTapped(this, e);
            }
        }


        //void EuropeManagers_ManagerTapped(object sender, ManagerIconTapEventArgs e)
        //{
        //    if (ManagerTapped != null)
        //    {
        //        ManagerTapped(this, e);
        //    }
        //}

        //void EuropeManagers_FlyoutOpened(object sender, System.EventArgs e)
        //{
        //    EuropeSubstitutes.Close();

        //    GridGrayedOut.Visibility = System.Windows.Visibility.Visible;

        //    EuropeSubstitutes.GridSlidOutState.IsHitTestVisible = false;
        //}

        //void EuropeManagers_FlyoutClosed(object sender, System.EventArgs e)
        //{
        //    GridGrayedOut.Visibility = System.Windows.Visibility.Collapsed;

        //    EuropeSubstitutes.GridSlidOutState.IsHitTestVisible = true;
        //}

        void EuropeSubstitutes_FlyoutOpened(object sender, System.EventArgs e)
        {
            //EuropeManagers.Close();

            GridGrayedOut.Visibility = System.Windows.Visibility.Visible;

            //EuropeManagers.GridSlidOutState.IsHitTestVisible = false;
        }

        void EuropeSubstitutes_FlyoutClosed(object sender, System.EventArgs e)
        {
            GridGrayedOut.Visibility = System.Windows.Visibility.Collapsed;
            //EuropeManagers.GridSlidOutState.IsHitTestVisible = true;
        }


        private void GridGrayedOut_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FlyoutSubstitutes.Close();
            //EuropeManagers.Close();
        }

        #endregion


        #region Helpers

        internal void Reload(PitchRecord pitch)
        {
            foreach (UserControl c in CanvasEurope.Children)
            {
                if (c is PlayerIconControl)
                {
                    PlayerIconControl icon = c as PlayerIconControl;
                    icon.Reload(pitch);
                }
            }

            //EuropeManagers.Reload();

            FlyoutSubstitutes.PlayerSubGoalkeper.IsEurope = true;
            FlyoutSubstitutes.PlayerSubDefender.IsEurope = true;
            FlyoutSubstitutes.PlayerSubMidfield.IsEurope = true;
            FlyoutSubstitutes.PlayerSubForward.IsEurope = true;

            FlyoutSubstitutes.Reload(pitch);
        }


        internal bool IsSubstitutesOpen()
        {
            return FlyoutSubstitutes.IsOpen();
        }

        internal void CloseSubstitutes()
        {
            FlyoutSubstitutes.Close();
        }

        //internal bool IsManagerOpen()
        //{
        //    return EuropeManagers.IsOpen();
        //}

        //internal void CloseManager()
        //{
        //    EuropeManagers.Close();
        //}

        #endregion
    }
}
