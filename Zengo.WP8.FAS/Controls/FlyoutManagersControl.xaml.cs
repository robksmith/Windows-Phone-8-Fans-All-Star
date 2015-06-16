
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
using System.Windows.Controls.Primitives;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class FlyoutManagersControl : UserControl
    {
        #region Events

        public event EventHandler<EventArgs> FlyoutOpened;
        public event EventHandler<EventArgs> FlyoutClosed;
        public event EventHandler<ManagerIconTapEventArgs> ManagerTapped;

        #endregion


        #region Properties

        public bool IsEurope { get; set; }

        #endregion


        #region Constructors

        public FlyoutManagersControl()
        {
            InitializeComponent();

            // Set up the event handlers for the players icons
            PlayerManager.ManagerTapped += PlayerManager_ManagerTapped;

            PlayerManager.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            PlayerManager.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;

            Loaded += ManagersFlyoutControl_Loaded;
        }

        #endregion


        #region Event Handlers

        void ManagersFlyoutControl_Loaded(object sender, RoutedEventArgs e)
        {
            // We re-use the same manager flyout control for europe and rotw so customise it here
            PlayerManager.IsEurope = IsEurope;

            if (IsEurope)
            {
                ImageButtonIn.Source = new BitmapImage(new Uri("/Images/btn_manager_eur.png", UriKind.Relative));
                ImageButtonOut.Source = new BitmapImage(new Uri("/Images/btn_manager.png", UriKind.Relative));
            }
            else
            {
                ImageButtonIn.Source = new BitmapImage(new Uri("/Images/btn_manager_row.png", UriKind.Relative));
                ImageButtonOut.Source = new BitmapImage(new Uri("/Images/btn_manager.png", UriKind.Relative));
            }

            Reload(null);
        }


        void PlayerManager_ManagerTapped(object sender, ManagerIconTapEventArgs e)
        {
            if (ManagerTapped != null)
            {
                ManagerTapped(this, e);
            }
        }


        private void ImageButtonIn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Open();
        }

        private void ImageButtonOut_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }

        #endregion


        internal void Reload(PitchRecord pitch)
        {
            PlayerManager.Reload(pitch);
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

        #region Animation

        private void PlayerIconControl_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 0, Y = -8 };
        }

        private void PlayerIconControl_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

        private void ImageButtonIn_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            if (IsEurope)
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager_eur_press.png", UriKind.Relative));
            }
            else
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager_row_press.png", UriKind.Relative));
            }
        }

        private void ImageButtonIn_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (IsEurope)
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager_eur.png", UriKind.Relative));
            }
            else
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager_row.png", UriKind.Relative));
            }
        }


        private void ImageButtonOut_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            if (IsEurope)
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager_press.png", UriKind.Relative));
            }
            else
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager_press.png", UriKind.Relative));
            }
        }

        private void ImageButtonOut_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (IsEurope)
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager.png", UriKind.Relative));
            }
            else
            {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_manager.png", UriKind.Relative));
            }
        }

        #endregion
    }
}
