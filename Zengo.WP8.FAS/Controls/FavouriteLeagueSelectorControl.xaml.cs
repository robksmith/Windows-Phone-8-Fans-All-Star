using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;

namespace Zengo.WP8.FAS.Controls
{
    public partial class FavouriteLeagueSelectorControl : UserControl
    {
        public class FavouriteLeagueBinding
        {
            public int FavId { get; set; }
            public string FavLeagueName { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> LeaguePressed;

        private LeagueAnswer league;

        public string NoSelectionMadeText { get; set; }

        #endregion

        #region Properties

        public LeagueAnswer League { get { return league; } }

        #endregion

        #region Constructors

        public FavouriteLeagueSelectorControl()
        {
            InitializeComponent();
            LeagueSelector.Tap += PlayerSelect_Tap;

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion

        #region EventHandlers

        private void PlayerSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (LeaguePressed != null)
            {
                LeaguePressed(this, new EventArgs());
            }
        }

        #endregion

        #region Helpers

        public void Refresh(LeagueAnswer leagueRecord)
        {
            league = leagueRecord;

            if (league != null)
            {
                var toBind = new FavouriteLeagueBinding {FavId = league.Id, FavLeagueName = league.LeagueName};
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new FavouriteLeagueBinding {FavId = 0, FavLeagueName = NoSelectionMadeText};
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        internal string SelectedId()
        {
            return league != null ? league.Id.ToString(CultureInfo.InvariantCulture) : "-1";
        }

        #endregion
    }
}
