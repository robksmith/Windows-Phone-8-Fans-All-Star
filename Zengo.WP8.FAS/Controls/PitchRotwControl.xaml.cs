
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
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PitchRotwControl : UserControl
    {
        #region Events

        public event EventHandler<PlayerIconTapEventArgs> PlayerTapped;
        //public event EventHandler<ManagerIconTapEventArgs> ManagerTapped;

        #endregion


        #region Constructors

        public PitchRotwControl()
        {
            InitializeComponent();

            //EuropeManagers.FlyoutOpened += EuropeManagers_FlyoutOpened;
            //EuropeManagers.FlyoutClosed += EuropeManagers_FlyoutClosed;

            FlyoutSubstitutes.FlyoutOpened += EuropeSubstitutes_FlyoutOpened;
            FlyoutSubstitutes.FlyoutClosed += EuropeSubstitutes_FlyoutClosed;

            foreach (UserControl c in CanvasRotw.Children)
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
            //EuropeManagers.ManagerTapped += Manager_Tapped;
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

        //void Manager_Tapped(object sender, ManagerIconTapEventArgs e)
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
            foreach (UserControl c in CanvasRotw.Children)
            {
                if (c is PlayerIconControl)
                {
                    PlayerIconControl icon = c as PlayerIconControl;
                    icon.Reload(pitch);
                }
            }

            //EuropeManagers.Reload();

            FlyoutSubstitutes.PlayerSubGoalkeper.IsEurope = false;
            FlyoutSubstitutes.PlayerSubDefender.IsEurope = false;
            FlyoutSubstitutes.PlayerSubMidfield.IsEurope = false;
            FlyoutSubstitutes.PlayerSubForward.IsEurope = false;

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
