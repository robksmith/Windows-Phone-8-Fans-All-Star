
#region Usings

using System;
using System.Windows.Controls;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Helpers;

#endregion

namespace Zengo.WP8.FAS.Controls.ListItems
{
    public class PlayerStatsTappedEventArgs : EventArgs
    {
        public PlayerRecord Player { get; set; }
    }

    public class PlayerVoteTappedEventArgs : EventArgs
    {
        public PlayerRecord Player { get; set; }
    }

    public partial class PlayerItemForSearch : UserControl
    {
        #region Events

        public event EventHandler<PlayerStatsTappedEventArgs> PlayerStatsTapped;
        public event EventHandler<PlayerVoteTappedEventArgs> PlayerVoteTapped;

        #endregion


        #region Constructors

        public PlayerItemForSearch()
        {
            InitializeComponent();

            TextBlockLastName.Tap += TextBlockLastName_Tap;

            TextBlockLastName.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            TextBlockLastName.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;


            ImageTick.Tap += ImageTick_Tap;

            ImageTick.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            ImageTick.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion


        #region Event Handlers

        void TextBlockLastName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PlayerStatsTapped != null)
            {
                if (DataContext is PlayerRecord)
                {
                    PlayerStatsTapped(this, new PlayerStatsTappedEventArgs() { Player = DataContext as PlayerRecord });
                }
            }
        }


        void ImageTick_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (PlayerVoteTapped != null)
            {
                if (DataContext is PlayerRecord)
                {
                    PlayerVoteTapped(this, new PlayerVoteTappedEventArgs() { Player = DataContext as PlayerRecord });
                }
            }
        }

        #endregion
    }
}