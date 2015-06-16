
#region Usings

using Zengo.WP8.FAS.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using Zengo.WP8.FAS.Views;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class VotePositionSelectControl : UserControl
    {
        #region Fields

        // what position have they selected
        PositionRecord positionToVoteFor;

        #endregion


        #region Events

        public event EventHandler<VoteConfirmEventArgs> VotePressed;
        public event EventHandler<EventArgs> CancelPressed;

        #endregion


        #region Constructors

        public VotePositionSelectControl()
        {
            InitializeComponent();

            // set up the slider
            //SliderVotesCount.Minimum = 1.0;
            //SliderVotesCount.Maximum = 100.0;
            //SliderVotesCount.ValueChanged += SliderVotesCount_ValueChanged;

            // Any player button taps, we want to know about
            PlayerPositionSelector.PlayerTapped += PlayerPositionSelector_PlayerTapped;
            PlayerPositionSelector.Reload();

            // Disable the cast vote button until they have selected a position
            ButtonCastVote.IsEnabled = false;
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// They have selected a position
        /// </summary>
        void PlayerPositionSelector_PlayerTapped(object sender, PlayerSelectionIconControl.PlayerTapEventArgs e)
        {
            // record the position 
            positionToVoteFor = e.VoteForPositionAt;

            // Light up the selected player
            PlayerPositionSelector.HighlightPosition(e.VoteForPositionAt);

            //MessageBox.Show(e.VoteForPositionAt.PositionId.ToString());

            ButtonCastVote.IsEnabled = true;
        }

        /// <summary>
        /// The slider has changed position - alter the number of votes
        /// </summary>
        //void SliderVotesCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    int votes = ((int)e.NewValue);

        //    TextblockVotes.Text = votes.ToString();

        //    ButtonCastVote.Content = "Cast Vote";
        //    if (votes > 1)
        //    {
        //        ButtonCastVote.Content = "Cast Votes";
        //    }
        //}

        /// <summary>
        /// The vote button has been pressed
        /// </summary>
        private void ButtonVote_Click(object sender, RoutedEventArgs e)
        {
            if (VotePressed != null)
            {
                PlayerListPage.VoteBoundType data = (PlayerListPage.VoteBoundType)DataContext;

                // Tell the containing page
                if (VotePressed != null)
                {
                    //VotePressed(this, new VoteConfirmEventArgs() { VoteCount = (int)SliderVotesCount.Value, Player = data.Player, Position = positionToVoteFor });
                    VotePressed(this, new VoteConfirmEventArgs() { VoteCount = 1, Player = data.Player, Position = positionToVoteFor });
                }
            }
        }

        /// <summary>
        /// Cancel has been pressed
        /// </summary>
        private void HyperlinkButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // tell the containing page
            if (CancelPressed != null)
            {
                CancelPressed(this, EventArgs.Empty);
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Given a zone, set up the control to display the selectable positions from that zone only
        /// </summary>
        internal void Configure(PlayerRecord player, PositionRecord position)
        {
            positionToVoteFor = position;

            if (position != null)
            {
                TextblockIns2.Text = "Please click the icon to alter your vote position:";
                ButtonCastVote.IsEnabled = true;
            }
            else
            {
                TextblockIns2.Text = "Please select a position for this player:";
                ButtonCastVote.IsEnabled = false;
            }

            PlayerPositionSelector.Configure(player, position);

            // If a position has already been chosen, highlight it
            if (position != null)
            {
                PlayerPositionSelector.HighlightPosition(position);
            }
        }

        #endregion
    }
}
