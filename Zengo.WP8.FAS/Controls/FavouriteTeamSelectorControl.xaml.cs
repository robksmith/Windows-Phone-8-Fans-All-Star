
#region Usings

using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using System;
using System.Windows.Controls;

#endregion


namespace Zengo.WP8.FAS.Controls
{
    public partial class FavouriteTeamSelectorControl : UserControl
    {
        public class FavouriteTeamBinding
        {
            public int FavId { get; set; }
            public string FavTeamName { get; set; }
            public string FavImage { get; set; }
        }

        #region Fields

        public event EventHandler<EventArgs> TeamPressed;
        
        ClubRecord club;

        public string NoSelectionMadeText { get; set; }

        #endregion


        #region Properties

        public ClubRecord Club { get { return club; } private set { } }

        #endregion


        #region Constructors

        public FavouriteTeamSelectorControl()
        {
            InitializeComponent();

            TeamSelector.Tap += TeamSelect_Tap;

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion


        #region Event Handlers

        void TeamSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (TeamPressed != null)
            {
                TeamPressed(this, new EventArgs());
            }
        }

        #endregion


        #region Helpers

        public void Refresh(ClubRecord club)
        {
            this.club = club;

            if (club != null)
            {
                var toBind = new FavouriteTeamBinding() { FavId = club.ClubId, FavTeamName = club.Name + string.Format(" ({0})", club.Country), FavImage = club.Image };
                LayoutRoot.DataContext = toBind;

                TextBlockDescription.Foreground = App.AppConstants.NormalTextColourBrush;
            }
            else
            {
                var toBind = new FavouriteTeamBinding() { FavId = 0, FavTeamName = NoSelectionMadeText, FavImage = "../Images/FavTeamNotChosen.png" };
                LayoutRoot.DataContext = toBind;

                TextBlockDescription.Foreground = App.AppConstants.WatermarkTextColourBrush;
            }

        }

        internal string SelectedId()
        {
            if (club != null)
            {
                return club.ClubId.ToString();
            }
            return "0";
        }

        #endregion
    }
}
