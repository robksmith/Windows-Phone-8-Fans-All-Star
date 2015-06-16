
#region Usings

using Zengo.WP8.FAS.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using Zengo.WP8.FAS.Views;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public class VoteConfirmEventArgs : EventArgs
    {
        public int VoteCount { get; set; }
        public PlayerRecord Player { get; set; }
        public PositionRecord Position { get; set; }
    }

    public partial class VoteConfirmControl : UserControl
    {
        #region Events

        public event EventHandler<VoteConfirmEventArgs> VotePressed;
        public event EventHandler<EventArgs> CancelPressed;

        #endregion


        #region Properties

        public VoteConfirmControl()
        {
            InitializeComponent();

            //SliderVotesCount.Minimum = 1.0;
            //SliderVotesCount.Maximum = 100.0;
            //SliderVotesCount.ValueChanged += SliderVotesCount_ValueChanged;
        }

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

        #endregion


        #region Event Handlers

        private void ButtonVote_Click(object sender, RoutedEventArgs e)
        {
            if (VotePressed != null)
            {
                PlayerListPage.VoteBoundType data = (PlayerListPage.VoteBoundType)DataContext;

                //VotePressed(this, new VoteConfirmEventArgs() { VoteCount = (int)SliderVotesCount.Value, Player = data.Player, Position = data.PositionVotedFor });
                VotePressed(this, new VoteConfirmEventArgs() { VoteCount = 1, Player = data.Player, Position = data.PositionVotedFor });
            }
        }

        private void HyperlinkButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (CancelPressed != null)
            {
                CancelPressed(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
