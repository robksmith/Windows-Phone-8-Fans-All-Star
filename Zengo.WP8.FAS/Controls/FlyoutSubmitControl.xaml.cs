
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
    public class SubmitIconTapEventArgs : EventArgs
    {
    }

    public partial class FlyoutSubmitControl : UserControl
    {
        #region Events

        public event EventHandler<SubmitIconTapEventArgs> ButtonTapped;

        #endregion


        #region Properties

        #endregion


        #region Constructors

        public FlyoutSubmitControl()
        {
            InitializeComponent();

            Loaded += ManagersFlyoutControl_Loaded;
        }

        #endregion


        #region Event Handlers

        void ManagersFlyoutControl_Loaded(object sender, RoutedEventArgs e)
        {
            ImageButtonIn.Source = new BitmapImage(new Uri("/Images/btn_submit.png", UriKind.Relative));
        }


        private void ImageButtonIn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ButtonTapped != null)
            {
                ButtonTapped(this, new SubmitIconTapEventArgs());
            }
        }

        private void ImageButtonOut_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }

        #endregion


        #region Animation

        //private void PlayerIconControl_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        //{
        //    ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 0, Y = -8 };
        //}

        //private void PlayerIconControl_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        //{
        //    ((UIElement)sender).RenderTransform = null;
        //}

        private void ImageButtonIn_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_submit_press.png", UriKind.Relative));
        }

        private void ImageButtonIn_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
                ((Image)sender).Source = new BitmapImage(new Uri("/Images/btn_submit.png", UriKind.Relative));
        }

        #endregion
    }
}
