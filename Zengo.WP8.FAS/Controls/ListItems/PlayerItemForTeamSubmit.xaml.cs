
#region Usings

using System;
using System.Windows.Controls;
using Zengo.WP8.FAS.Models;

#endregion

namespace Zengo.WP8.FAS.Controls.ListItems
{
    public partial class PlayerItemForTeamSubmit : UserControl
    {
        #region Events

        public event EventHandler<PlayerStatsTappedEventArgs> PlayerStatsTapped;
        public event EventHandler<PlayerVoteTappedEventArgs> PlayerVoteTapped;

        #endregion


        #region Constructors

        public PlayerItemForTeamSubmit()
        {
            InitializeComponent();
        }

        #endregion


        #region Event Handlers

        #endregion
    }
}