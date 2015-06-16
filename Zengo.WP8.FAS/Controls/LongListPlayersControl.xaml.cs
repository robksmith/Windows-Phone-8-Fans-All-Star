
#region Usings

using Zengo.WP8.FAS.Controls.ListItems;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class LongListPlayersControl : UserControl
    {
        public class PlayerSelectionChangedEventArgs : EventArgs
        {
            public PlayerRecord Player { get; set; }
        }


        #region Events

        public event EventHandler<PlayerSelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<PlayerStatsTappedEventArgs> PlayerStatsTapped;
        public event EventHandler<PlayerVoteTappedEventArgs> PlayerVoteTapped;

        #endregion


        #region Properties

        private bool forFreeEntryPage;
        public bool ForFreeEntryPage
        {
            get { return forFreeEntryPage; }
            set
            {
                forFreeEntryPage = value;
                if (forFreeEntryPage)
                {
                    PlayersLongList.ItemTemplate = (DataTemplate)Resources["playersItemForFreeEntryTemplate"];
                    PlayersLongList.SelectionChanged += PlayersLongListOnSelectionChanged;
                }
                else
                {
                    PlayersLongList.ItemTemplate = (DataTemplate)Resources["playersItemForSearchTemplate"];
                }
            }
        }

        private bool forTeamSubmit;
        public bool ForTeamSubmit
        {
            get { return forTeamSubmit; }
            set
            {
                forTeamSubmit = value;
                if (forTeamSubmit)
                {
                    PlayersLongList.ItemTemplate = (DataTemplate)Resources["playersItemForTeamSubmitTemplate"];
                    //PlayersLongList.IsFlatList = true;
                }
                else
                {
                    PlayersLongList.ItemTemplate = (DataTemplate)Resources["playersItemForTeamSubmitTemplate"];
                }
            }
        }

        #endregion


        #region Constructors

        public LongListPlayersControl()
        {
            InitializeComponent();
        }

        #endregion


        #region Events

        public void PlayerItemViewOnPage_PlayerStatsTapped(object sender, PlayerStatsTappedEventArgs e)
        {
            if (PlayerStatsTapped != null)
            {
                PlayerStatsTapped(this, e);
            }
        }

        public void PlayerItemViewOnPage_PlayerVoteTapped(object sender, PlayerVoteTappedEventArgs e)
        {
            if (PlayerVoteTapped != null)
            {
                PlayerVoteTapped(this, e);
            }
        }

        private void PlayersLongListOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new PlayerSelectionChangedEventArgs { Player = (PlayerRecord)PlayersLongList.SelectedItem });
            }
        }

        #endregion


        #region Animation

        private void PlayerItemViewOnPage_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 2, Y = 2 };
        }

        private void PlayerItemViewOnPage_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

        #endregion
    }
}
