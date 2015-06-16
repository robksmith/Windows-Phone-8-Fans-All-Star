using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WepApi;

namespace Zengo.WP8.FAS.Controls
{
    public class PlayerSelectorChangedStateEventArgs : EventArgs
    {
        public bool Enabled { get; set; }
    }

    public partial class FreeEntryControl : INotifyPropertyChanged
    {
        #region Events

        public class FreeEntryCompletedEventArgs : EventArgs
        {
            public bool Success { get; set; }

            public string Message { get; set; }
        }

        public event EventHandler<EventArgs> FreeEntryStarting;
        public event EventHandler<FreeEntryCompletedEventArgs> FreeEntryCompleted;

        public event EventHandler<EventArgs> FavouritePlayerPressed;
        public event EventHandler<EventArgs> FavouriteLeaguePressed;
        public event EventHandler<EventArgs> FavouriteStadiumPressed;
        public event EventHandler<EventArgs> FavouriteSportPressed;

        #endregion

        #region Fields

        private readonly UserApi userApi;
        
        public static FreeEntryPage Page { get; set; }

        #endregion

        #region Properties

        private string toggleBoxText;
        public string ToggleBoxText
        {
            get { return toggleBoxText; }
            set
            {
                if (toggleBoxText != value)
                {
                    toggleBoxText = value;
                    OnPropertyChanged("ToggleBoxText");
                }
            }
        }
        #endregion

        #region Constructors

        public FreeEntryControl()
        {
            InitializeComponent();

            userApi = new UserApi();
            userApi.FreeEntryCompleted += UserApiOnFreeEntryCompleted;

            ResetErrors();

            TitleFavPlayer.Title = AppResources.FavouritePlayer;
            TitleMajorLeague.Title = AppResources.FavouriteLeague;
            TitleStadium.Title = AppResources.FavouriteStadium;
            TitleOtherSport.Title = AppResources.FavouriteSport;
            TitlePlayFootball.Title = AppResources.DoYouPlayFootball;

           // ToggleSwitch.DataContext = this;
            FavPlayer.PlayerPressed += FavPlayerOnPlayerPressed;
            FavLeague.LeaguePressed += FavLeagueOnLeaguePressed;
            FavStadium.StadiumPressed += FavStadiumOnStadiumPressed;
            FavSport.SportPressed += FavSportOnSportPressed;

        }

        private void UserApiOnFreeEntryCompleted(object sender, FreeEntryEventArgs e)
        {
            if( e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good )
            {
                // Update Records
                App.ViewModel.DbViewModel.MergeVotes(e.ServerResponse.Response.Votes);
                App.ViewModel.DbViewModel.FreeQuestionAnswered();

                if (FreeEntryCompleted != null)
                {
                    FreeEntryCompleted(this, new FreeEntryCompletedEventArgs() { Success = true } );
                }
            }
        }

        #endregion

        public void ResetErrors()
        {
            TitleFavPlayer.ClearError();
            TitleMajorLeague.ClearError();
            TitleOtherSport.ClearError();
            TitlePlayFootball.ClearError();
            TitleStadium.ClearError();
        }

        #region Press Events

        private void FavPlayerOnPlayerPressed(object sender, EventArgs eventArgs)
        {
            if (FavouritePlayerPressed != null)
            {
                FavouritePlayerPressed(this, new EventArgs());
            }
        }

        private void FavLeagueOnLeaguePressed(object sender, EventArgs eventArgs)
        {
            if (FavouriteLeaguePressed != null)
            {
                FavouriteLeaguePressed(this, new EventArgs());
            }
        }

        private void FavStadiumOnStadiumPressed(object sender, EventArgs e)
        {
            if (FavouriteStadiumPressed != null)
            {
                FavouriteStadiumPressed(this, new EventArgs());
            }
        }

        private void FavSportOnSportPressed(object sender, EventArgs e)
        {
            if (FavouriteSportPressed != null)
            {
                FavouriteSportPressed(this, new EventArgs());
            }
        }

        private void ToggleSwitch_OnChecked(object sender, RoutedEventArgs e)
        {
            ToggleBoxText = AppResources.Yes;
        }

        private void ToggleSwitch_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ToggleBoxText = AppResources.No;
        }

        #endregion

        public void SetFavourites(PlayerRecord favPlayer, LeagueAnswer favLeague, StadiumAnswer favStadium, SportAnswer favSport)
        {
            FavPlayer.Refresh(favPlayer);
            FavLeague.Refresh(favLeague);
            FavSport.Refresh(favSport);
            FavStadium.Refresh(favStadium);
        }


        public void FreeEntry()
        {
            bool pageValid = false;
            Page.Focus();

            ResetErrors();

            if (Validation.ValidateSelectorChosenForFreeEntry(TitleFavPlayer, FavPlayer.SelectedId(), AppResources.FavouritePlayerWatermark))
            {
                if (Validation.ValidateSelectorChosenForFreeEntry(TitleMajorLeague, FavLeague.SelectedId(), AppResources.FavouriteLeagueWatermark))
                {
                    if (Validation.ValidateSelectorChosenForFreeEntry(TitleStadium, FavStadium.SelectedId(), AppResources.FavouriteStadiumWatermark))
                    {
                        if (Validation.ValidateSelectorChosenForFreeEntry(TitleOtherSport, FavSport.SelectedId(), AppResources.FavouriteSportWatermark))
                        {
                            pageValid = true;
                        }
                    }
                }
            }

            if (pageValid)
            {
                string favPlayer = FavPlayer.SelectedId();
                string favLeague = FavLeague.SelectedId();
                string favStadium = FavStadium.SelectedId();
                string favSport = FavSport.SelectedId();
                string playFootball = "";// ToggleSwitch.IsChecked == true ? "1" : "0";

                if (FreeEntryStarting != null)
                {
                    FreeEntryStarting(this, new EventArgs());
                }

                userApi.FreeEntry(App.ViewModel.DbViewModel.CurrentUser.UserId, favPlayer, favLeague, favStadium, favSport, playFootball);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
