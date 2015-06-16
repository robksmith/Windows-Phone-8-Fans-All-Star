
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
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Resources;

#endregion

/*
Jason has confirmed that the selectable column works like this:

value = 0,  only show the country in the list where you filter players by country
value = 1,  only show the country in the list that appears in user details or registration forms (my country), NOT in the list where the players are flitered
value = 2,  show the country in all lists

eg.

filters:               selectable != 1
favourite team: selectable > 0

 * 
 * 
 * 
It's an int and takes a values of 0,1,2:

0 - Country is selectable only in player search
1 - Country is selectable only in user account
2 - Country is selectable in both

So your player search country list is found with the condition:
(selectable==0||selectable==2)

And your account player list is found with the condition:
(selectable==1||selectable==2)

 
 */

namespace Zengo.WP8.FAS.Controls
{
    public partial class MyCountrySelectorControl : UserControl
    {
        public class MyCountryBinding
        {
            public int CountryId { get; set; }
            public string CountryName { get; set; }
            public string CountryImage { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> CountryPressed;
        private CountryRecord country;

        #endregion

        #region Properties

        public string NoSelectionMadeText { get; set; }
        public CountryRecord Country { get { return country; } private set { } }

        #endregion


        #region Constructors

        public MyCountrySelectorControl()
        {
            InitializeComponent();

            CountrySelector.Tap += CountrySelect_Tap;

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion


        #region Event Handlers

        void CountrySelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (CountryPressed != null)
            {
                CountryPressed(this, new EventArgs());
            }
        }

        #endregion


        #region Helpers

        public void Refresh(CountryRecord country)
        {
            this.country = country;

            if ( country != null )
            {
                var toBind = new MyCountryBinding() { CountryId = country.CountryId, CountryName = country.Name, CountryImage = country.ImageToRender };
                LayoutRoot.DataContext = toBind;

                TextBlockDescription.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new MyCountryBinding() { CountryId = 0, CountryName = NoSelectionMadeText, CountryImage = "../Images/CountryNotChosen.png" };
                LayoutRoot.DataContext = toBind;

                TextBlockDescription.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

        }

        #endregion


        internal string SelectedId()
        {
            if (country != null)
            {
                return country.CountryId.ToString();
            }
            return "0";
        }
    }
}
