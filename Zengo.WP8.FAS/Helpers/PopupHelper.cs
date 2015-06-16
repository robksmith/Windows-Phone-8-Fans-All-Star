
#region Usings

using Zengo.WP8.FAS.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Threading;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class PopupMessage
    {
        public UserControl Control { get; set; }
        public TimeSpan TimeLength { get; set; }

        public PopupMessage(UserControl control)
        {
            Control = control;
            TimeLength = TimeSpan.FromSeconds(3.0);
        }

        public PopupMessage(UserControl control, TimeSpan timeSpan)
        {
            Control = control;
            TimeLength = timeSpan;
        }
    }

    public class PopupHelper
    {
        #region Fields

        Popup popupControl;
        PopupMessageControl messageControl;
        Queue<PopupMessage> popupMessages = new Queue<PopupMessage>();
        DispatcherTimer timer;
        TimeSpan intervalDelay = new TimeSpan(0, 0, 0, 0, 500);

        Storyboard fadeInAnimation;
        Storyboard fadeOutAnimation;

        #endregion


        #region Properties

        public Queue<PopupMessage> PopupMessages { get { return popupMessages; } }

        #endregion


        #region Constructors

        public PopupHelper()
        {
            // Create the popups
            popupControl = new Popup();
            messageControl = new PopupMessageControl();

            popupControl.Child = messageControl;
            popupControl.VerticalOffset = 28;

            // Create the timer
            timer = new DispatcherTimer();

            // Turn on the first popup and timer
            FetchMessageAndDisplayPopup();
        }

        #endregion


        #region Timer Events

        /// <summary>
        /// A popup has been displayed for the relevant amount of time - so start the interval timer
        /// </summary>
        void timer_PopupEnd_Tick(object sender, EventArgs e)
        {
            // close the popup
            if (popupControl.IsOpen)
            {
                fadeOutAnimation.Begin();
            }

            // stop the timer
            timer.Stop();
            timer.Tick -= new EventHandler(timer_PopupEnd_Tick);

            // start a timer for the interval
            if (intervalDelay > TimeSpan.Zero)
            {
                StartIntervalTimer(intervalDelay);
            }
            else
            {
                FetchMessageAndDisplayPopup();
            }
        }

        void fadeOut_Completed(object sender, EventArgs e)
        {
            popupControl.IsOpen = false;
        }

        /// <summary>
        /// An interval has been displayed for the relevant amount of time
        /// </summary>
        void timer_IntervalEnd_Tick(object sender, EventArgs e)
        {
            // stop the timer
            timer.Stop();
            timer.Tick -= new EventHandler(timer_IntervalEnd_Tick);

            // Get next message
            FetchMessageAndDisplayPopup();
        }

        #endregion


        #region Popup Stuff

        private void StartPopupTimer(TimeSpan length)
        {
            if (timer != null)
            {
                timer.Interval = length;
                timer.Tick += new EventHandler(timer_PopupEnd_Tick);
                timer.Start();
            }
        }

        private void StartIntervalTimer(TimeSpan length)
        {
            if (timer != null)
            {
                timer.Interval = length;
                timer.Tick += new EventHandler(timer_IntervalEnd_Tick);
                timer.Start();
            }
        }

        private void FetchMessageAndDisplayPopup()
        {
            bool messageAvailable = (PopupMessages.Count > 0);

            // If a message is available
            if (messageAvailable && !popupControl.IsOpen)
            {
                PopupMessage message = PopupMessages.Dequeue();

                popupControl.Child = message.Control;

                // Create the animations
                fadeInAnimation = CreateFadeInOutAnimation(0.0, 1.0, message.Control);
                fadeOutAnimation = CreateFadeInOutAnimation(1.0, 0.0, message.Control);

                fadeOutAnimation.Completed += fadeOut_Completed;

                popupControl.VerticalOffset = 28;

                // open the control
                popupControl.IsOpen = true;
                fadeInAnimation.Begin();

                // Start the timer
                StartPopupTimer(message.TimeLength);
            }
            else
            {
                StartIntervalTimer(intervalDelay);
            }
        }

        private void KillAllPopups()
        {
            popupControl.IsOpen = false;

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        #endregion


        #region Animation

        Storyboard CreateFadeInOutAnimation(double from, double to, UserControl target)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation();
            fadeAnimation.From = from;
            fadeAnimation.To = to;
            fadeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            Storyboard.SetTarget(fadeAnimation, target);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("Opacity"));

            sb.Children.Add(fadeAnimation);

            return sb;
        }

        #endregion

    }
}
