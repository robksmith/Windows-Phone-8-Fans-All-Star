
#region Usings

using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

#endregion


namespace Zengo.WP8.FAS.Controls
{
    public partial class PlayerSelectionIconControl : UserControl
    {
        public class PlayerTapEventArgs : EventArgs
        {
            public ZoneRecord Zone { get; set; }
            public PositionRecord VoteForPositionAt { get; set; }
        }

        #region Events

        public event EventHandler<PlayerTapEventArgs> PlayerTapped;

        #endregion


        #region Fields

        private int positionId;

        #endregion


        #region Properties

        public int PositionId { get { return positionId; } set { positionId = value; } }

        //public string AlternativeDescription { get; set; }

        public double PlayerOpacity { get { return ImagePlayer.Opacity; } set { ImagePlayer.Opacity = value; } }

        #endregion


        #region Constructors

        public PlayerSelectionIconControl()
        {
            InitializeComponent();

            ImagePlayer.Tap += ImagePlayer_Tap;
        }

        #endregion


        #region Event Handlers

        void ImagePlayer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PlayerTapped != null)
            {
                ZoneRecord zone = App.ViewModel.DbViewModel.ZoneFromPosition(positionId);
                PositionRecord pos = App.ViewModel.DbViewModel.Position(positionId);

                PlayerTapped(this, new PlayerTapEventArgs() { Zone = zone, VoteForPositionAt = pos });
            }
        }

        #endregion


        #region Helpers

        internal void Reload()
        {
            // Set the outer colour of the player image box
            if (PositionId <= 16)
            {
                //RectangleImage.Stroke = new SolidColorBrush(Colors.Blue);
                ImagePlayer.Source = new BitmapImage(new Uri("/Images/sta_player_eur.png", UriKind.Relative));
            }
            else
            {
                ImagePlayer.Source = new BitmapImage(new Uri("/Images/sta_player_row.png", UriKind.Relative));
                //RectangleImage.Stroke = new SolidColorBrush(Colors.Red);
            }

            // Fill in the position name - this is a description
            if (PositionId > 0)
            {
                PositionRecord pos = App.ViewModel.DbViewModel.Position(PositionId);
                if (pos != null)
                {
                    // Dont use the position name - look up a different name
                    TextPlayer.Text = PitchConstants.KeyToName(pos.Key);
                    //TextPlayer.Text = pos.Name;
                    //if (!string.IsNullOrEmpty(AlternativeDescription))
                    //{
                    //    TextPlayer.Text = AlternativeDescription;
                    //}
                }
            }
        }

        #endregion
    }
}
