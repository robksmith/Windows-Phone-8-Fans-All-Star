
#region Usings

using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.ViewModels;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

#endregion


namespace Zengo.WP8.FAS.Controls
{
    public class PlayerIconTapEventArgs : EventArgs
    {
        public ZoneRecord Zone { get; set; }
        public PositionRecord VoteForPositionAt { get; set; }
    }

    public class ManagerIconTapEventArgs : EventArgs
    {
        public Boolean IsEurope { get; set; }
    }

    public partial class PlayerIconControl : UserControl
    {
        #region Events

        public event EventHandler<PlayerIconTapEventArgs> PlayerTapped;
        public event EventHandler<ManagerIconTapEventArgs> ManagerTapped;

        #endregion


        #region Fields

        private string positionKey;
        private bool isEurope;

        #endregion


        #region Properties

        public string PositionKey { get { return positionKey; } set { positionKey = value; } }
        public bool IsEurope { get { return isEurope; } set { isEurope = value; } }
        //public string AlternativeDescription { get; set; }

        #endregion


        #region Constructors

        public PlayerIconControl()
        {
            InitializeComponent();

            GridIcon.Tap += GridIcon_Tap;
        }


        #endregion


        #region Event Handlers

        void GridIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ManagerTapped != null)
            {
                if (positionKey == "MG")
                {
                    ManagerTapped(this, new ManagerIconTapEventArgs() { IsEurope = isEurope });
                }
            }

            if (PlayerTapped != null)
            {
                PositionRecord pos = App.ViewModel.DbViewModel.Position(positionKey, isEurope);
                if (pos != null)
                {
                    PlayerTapped(this, new PlayerIconTapEventArgs() { Zone = pos.Zone, VoteForPositionAt = pos });
                }
                else
                {
#if DEBUG
                    if (App.AppConstants.ApplyingUpdates)
                    {
                        MessageBox.Show("Applying update", "Wait", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("There is no such position key as " + positionKey, "Error", MessageBoxButton.OK);
                    }
#endif
                }
            }

        }

        private void PlayerIconControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        #endregion


        #region Helpers

        internal PositionRecord LoadDefault()
        {
            // Fill in with the default empty star image for Europe or Rotw?
            if (isEurope)
            {
                ImagePlayer.Source = new BitmapImage(new Uri("/Images/sta_player_eur.png", UriKind.Relative));
            }
            else
            {
                ImagePlayer.Source = new BitmapImage(new Uri("/Images/sta_player_row.png", UriKind.Relative));
            }

            // What position is this id associated with
            PositionRecord position = App.ViewModel.DbViewModel.Position(PositionKey, isEurope);
            if (position == null)
            {
                // If we can't find a position just assume it is the manager and return
                if (isEurope)
                {
                    TextPlayer.Text = App.AppConstants.EuropeManager;
                }
                else
                {
                    TextPlayer.Text = App.AppConstants.RotwManager;
                }
                return null;
            }

            // Fill in the position description
            TextPlayer.Text = PitchConstants.KeyToName(positionKey);

            return position;
        }


        internal void Reload(PitchRecord pitch)
        {
            PositionRecord position = LoadDefault();
            if (position == null)
            {
                return;
            }

            // If the user is logged in and has votes
            if (App.ViewModel.DbViewModel.IsLoggedOn())
            {
                PlayerRecord player = null;
                if (pitch != null)
                {
                    // === Historical data using the pitch ===

                    // Find the corresponding position in the pitch, so we can get the player
                    PitchLocationRecord location = pitch.FindLocation(position);

                    if (location != null)
                    {
                        player = App.ViewModel.DbViewModel.Player(location.PlayerId);
                    }
                }
                else
                {
                    // === Current Data ===

                    // Get this icons position
                    if (App.ViewModel.ShowMyTeam)
                    {
                        player = App.ViewModel.DbViewModel.LastUniquePlayerByPosition(position);
                    }
                    //else
                    //{
                    //    player = App.ViewModel.DbViewModel.LastVoteCastByPosition(position);
                    //}
                }

                if (player != null)
                {
                    // Set the full star image for Europe or Rotw
                    if (position.IsEurope)
                    {
                        ImagePlayer.Source = new BitmapImage(new Uri("/Images/sta_player_eur_full.png", UriKind.Relative));
                    }
                    else
                    {
                        ImagePlayer.Source = new BitmapImage(new Uri("/Images/sta_player_row_full.png", UriKind.Relative));
                    }

                    // Set the last name
                    TextPlayer.Text = App.ViewModel.DbViewModel.ChoosePlayerName(player);

                    // Set the players name - and show votes per position if required
                    //if (App.ViewModel.ShowVotesPerPosition)
                    //{
                    //    int votesCast = App.ViewModel.DbViewModel.CountVotesCastByPosition(position);
                    //    if (votesCast > 0)
                    //    {
                    //        TextPlayer.Text += string.Format(" ({0})", votesCast.ToString());
                    //    }
                    //}
                }
            }
        }

        #endregion

    }
}
