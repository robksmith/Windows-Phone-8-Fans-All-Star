
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
using System.Windows.Threading;

#endregion

namespace Zengo.WP8.FAS
{
    public partial class TestPage3 : PhoneApplicationPage
    {
        #region Fields

        // The delay timer
        DispatcherTimer timer;

        // Delay between polling
        TimeSpan intervalDelay = new TimeSpan(0, 0, 0, 0, 50);

        // Random numbers
        Random random = new Random();

        // time counter
        float counter = 0.0f;

        #endregion


        public TestPage3()
        {
            InitializeComponent();

            // To simulate an continuous input, have had to set up a timer
            StartTimer();
        }



        #region Helpers

        /// <summary>
        /// Initialise the timer
        /// </summary>
        private void StartTimer()
        {
            timer = new System.Windows.Threading.DispatcherTimer();
            if (timer != null)
            {
                timer.Interval = intervalDelay;
                timer.Tick += new EventHandler(timer_End_Tick);
                timer.Start();
            }
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= new EventHandler(timer_End_Tick);
                timer = null;
            }
        }

        #endregion


        #region Timer and Api Events

        /// <summary>
        /// Update the wave
        /// </summary>
        void timer_End_Tick(object sender, EventArgs e)
        {
            // Pass the gain into the wave update routine - for this test, pass in the sine of time so it bobs up and down
            WaveControl.Update(Math.Sin(counter));

            // increment our dummy time
            counter += 0.2f;
        }

        #endregion

    }
}