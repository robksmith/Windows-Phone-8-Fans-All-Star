using System;
using System.Globalization;
using System.Windows.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;

namespace Zengo.WP8.FAS.Controls
{
    public partial class FavouriteStadiumSelectorControl : UserControl
    {
        public class FavouriteStadiumBinding
        {
            public int FavId { get; set; }
            public string FavStadiumName { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> StadiumPressed;

        private StadiumAnswer stadium;

        public string NoSelectionMadeText { get; set; }

        #endregion

        #region Properties

        public StadiumAnswer League { get { return stadium; } }

        #endregion

        #region Constructors

        public FavouriteStadiumSelectorControl()
        {
            InitializeComponent();
            StadiumSelector.Tap += StadiumSelect_Tap;

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion

        #region EventHandlers

        private void StadiumSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (StadiumPressed != null)
            {
                StadiumPressed(this, new EventArgs());
            }
        }

        #endregion

        #region Helpers

        public void Refresh(StadiumAnswer stadiumRecord)
        {
            stadium = stadiumRecord;

            if (stadium != null)
            {
                var toBind = new FavouriteStadiumBinding() { FavId = stadium.Id, FavStadiumName = stadium.StadiumName };
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new FavouriteStadiumBinding() {FavId = 0, FavStadiumName = NoSelectionMadeText};
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        internal string SelectedId()
        {
            return stadium != null ? stadium.Id.ToString(CultureInfo.InvariantCulture) : "-1";
        }

        #endregion
    }
}
