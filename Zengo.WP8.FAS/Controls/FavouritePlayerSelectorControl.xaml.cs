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
    public partial class FavouritePlayerSelectorControl : UserControl
    {
        public class FavouritePlayerBinding
        {
            public int FavId { get; set; }
            public string FavPlayerName { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> PlayerPressed;

        PlayerRecord player;

        public string NoSelectionMadeText { get; set; }

        #endregion

        #region Properties

        public PlayerRecord Player { get { return player; } }

        #endregion

        #region Constructors

        public FavouritePlayerSelectorControl()
        {
            InitializeComponent();

            PlayerSelector.Tap += PlayerSelect_Tap;

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion

        #region EventHandlers

        void PlayerSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PlayerPressed != null)
            {
                PlayerPressed(this, new EventArgs());
            }
        }

        #endregion

        #region Helpers

        public void Refresh(PlayerRecord playerRecord)
        {
            player = playerRecord;

            if (player != null)
            {
                var toBind = new FavouritePlayerBinding {FavId = player.PlayerId, FavPlayerName = playerRecord.FirstName + " " + playerRecord.LastName};
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new FavouritePlayerBinding {FavId = 0, FavPlayerName = NoSelectionMadeText};
                LayoutRoot.DataContext = toBind;

                TextBlockName.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }
        }

        internal string SelectedId()
        {
            return player != null ? player.PlayerId.ToString(CultureInfo.InvariantCulture) : "-1";
        }

        #endregion
    }
}
