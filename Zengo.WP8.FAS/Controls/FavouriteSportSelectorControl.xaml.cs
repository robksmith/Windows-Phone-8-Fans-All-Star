using System;
using System.Globalization;
using System.Windows.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;

namespace Zengo.WP8.FAS.Controls
{
    public partial class FavouriteSportSelectorControl : UserControl
    {
        public class FavouriteSportBinding
        {
            public int FavId { get; set; }
            public string FavSportName { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> SportPressed;

        private SportAnswer sport;

        public string NoSelectionMadeText { get; set; }

        #endregion

        #region Properties

        public SportAnswer League { get { return sport; } }

        #endregion

        #region Constructors

        public FavouriteSportSelectorControl()
        {
            InitializeComponent();
            SportSelector.Tap += SportSelect_Tap;

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion

        #region EventHandlers

        private void SportSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (SportPressed != null)
            {
                SportPressed(this, new EventArgs());
            }
        }

        #endregion

        #region Helpers

        public void Refresh(SportAnswer sportRecord)
        {
            sport = sportRecord;

            if (sport != null)
            {
                var toBind = new FavouriteSportBinding { FavId = sport.Id, FavSportName = sport.SportName };
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new FavouriteSportBinding() {FavId = 0, FavSportName = NoSelectionMadeText};
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        internal string SelectedId()
        {
            return sport != null ? sport.Id.ToString(CultureInfo.InvariantCulture) : "-1";
        }

        #endregion
    }
}
