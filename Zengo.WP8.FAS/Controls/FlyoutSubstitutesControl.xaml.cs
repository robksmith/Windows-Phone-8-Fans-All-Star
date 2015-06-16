
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
using System.Windows.Media.Imaging;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class FlyoutSubstitutesControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> FlyoutOpened;
        public event EventHandler<EventArgs> FlyoutClosed;
        public event EventHandler<PlayerIconTapEventArgs> PlayerTapped;
        
        #endregion


        #region Properties

        public bool IsEurope { get; set; }

        #endregion


        #region Constructors

        public FlyoutSubstitutesControl()
        {
            InitializeComponent();

            // Set up the event handlers for the players icons
            PlayerSubGoalkeper.PlayerTapped += Player_Tapped;
            PlayerSubDefender.PlayerTapped += Player_Tapped;
            PlayerSubMidfield.PlayerTapped += Player_Tapped;
            PlayerSubForward.PlayerTapped += Player_Tapped;

            PlayerSubGoalkeper.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            PlayerSubGoalkeper.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;

            PlayerSubDefender.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            PlayerSubDefender.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;

            PlayerSubMidfield.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            PlayerSubMidfield.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;

            PlayerSubForward.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            PlayerSubForward.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;

            //Loaded += FlyoutSubstitutesControl_Loaded;
        }
        
        #endregion


        #region Event Handlers

        //void FlyoutSubstitutesControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //// We re-use the same subs flyout control for europe and rotw so customise it here
        //    //PlayerSubGoalkeper.IsEurope = IsEurope;
        //    //PlayerSubDefender.IsEurope = IsEurope;
        //    //PlayerSubMidfield.IsEurope = IsEurope;
        //    //PlayerSubForward.IsEurope = IsEurope;


        //    //Reload(null);
        //}

        void Player_Tapped(object sender, PlayerIconTapEventArgs e)
        {
            if (PlayerTapped != null)
            {
                PlayerTapped(this, e);
            }
        }

        private void SubsButtonIn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Open();
        }

        private void SubsButtonOut_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }

        #endregion


        #region Helpers

        internal void Reload(PitchRecord pitch)
        {
            PlayerSubGoalkeper.Reload(pitch);
            PlayerSubDefender.Reload(pitch);
            PlayerSubMidfield.Reload(pitch);
            PlayerSubForward.Reload(pitch);
        }


        public bool IsOpen()
        {
            return GridSlidOutState.Visibility == System.Windows.Visibility.Visible;
        }

        private void Open()
        {
            LayoutRoot.Height = 800;

            GridSlidOutState.Visibility = System.Windows.Visibility.Visible;
            GridSlidInState.Visibility = System.Windows.Visibility.Collapsed;

            if (FlyoutOpened != null)
            {
                FlyoutOpened(this, new EventArgs());
            }
        }

        internal void Close()
        {
            LayoutRoot.Height = 136;

            GridSlidOutState.Visibility = System.Windows.Visibility.Collapsed;
            GridSlidInState.Visibility = System.Windows.Visibility.Visible;

            if (FlyoutClosed != null)
            {
                FlyoutClosed(this, new EventArgs());
            }
        }

        #endregion


        #region Animation

        private void SubsButtonIn_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_subs_press.png", UriKind.Relative)); 
        }

        private void SubsButtonIn_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_subs.png", UriKind.Relative));
        }

        #endregion

    }
}
