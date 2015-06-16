
#region Usings

using System.Text;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WepApi.Api;
using Zengo.WP8.FAS.WepApi;
using System.Collections.Generic;
using Zengo.WP8.FAS.Models;

#endregion


namespace Zengo.WP8.FAS
{
    public partial class PitchPage : PhoneApplicationPage
    {
        #region Fields

        #endregion


        #region Constructors

        public PitchPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
           // DataContext = App.ViewModel;
        }

        #endregion


        #region Page Event Handlers

        /// <summary>
        /// This also gets run when we navigate back from the clubs list
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            EuropeROTWHeaderControl.PageName = AppResources.ROTWName;
            EuropePivotHeaderControl.PageName = AppResources.EuropeName;
            EuropeFullHeaderControl.PageName = AppResources.FullViewPivot;

            // Get the required bid id an initialise the browser
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("Pitch"))
            {
                string pitch = parameters["Pitch"];

                PitchRecord pitchRecord = App.ViewModel.DbViewModel.GetPitch(Convert.ToInt32(pitch));

                DataContext = pitchRecord;

                PitchEurope.Reload(pitchRecord);
                PitchRotw.Reload(pitchRecord);
                PitchFull.Reload(pitchRecord);
            }
        }

        #endregion


        #region Back Key

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (PitchEurope.IsSubstitutesOpen())
            {
                PitchEurope.CloseSubstitutes();
                e.Cancel = true;
            }

            if (PitchRotw.IsSubstitutesOpen())
            {
                PitchRotw.CloseSubstitutes();
                e.Cancel = true;
            }
        }

        #endregion
    }
}