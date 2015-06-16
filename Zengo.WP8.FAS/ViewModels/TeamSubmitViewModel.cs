
#region Usings

using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS.ViewModels
{
    public class TeamSubmitViewModel : INotifyPropertyChanged
    {
        public class PlayerPosition
        {
            public PlayerRecord Player { get; set; }
            public PositionRecord Position { get; set; }
        }


        #region Events

        public event EventHandler<SubmitTeamEventArgs> Completed;

        #endregion


        #region Fields

        private bool isLoading;
        private bool isSubmittedFailure;
        private ObservableCollection<PlayerPosition> players;
        private ObservableCollection<CurrentPitchLocationRecord> pitch;

        #endregion


        #region Properties

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                NotifyPropertyChanged("IsLoading");
                isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public bool IsSubmittedFailure
        {
            get { return isSubmittedFailure; }
            set
            {
                NotifyPropertyChanged("IsSubmittedFailure");
                isSubmittedFailure = value;
                NotifyPropertyChanged("IsSubmittedFailure");
            }
        }

        public ObservableCollection<PlayerPosition> Players
        {
            get { return players; }
            set
            {
                if (players != value)
                {
                    players = value;
                    NotifyPropertyChanged("Players");
                }
            }
        }

        #endregion


        #region Constructors

        public TeamSubmitViewModel()
        {
            players = new ObservableCollection<PlayerPosition>();
        }

        internal void SetupPlayers(ObservableCollection<CurrentPitchLocationRecord> pitch)
        {
            this.pitch = pitch;

            Players = App.ViewModel.DbViewModel.PlayersList(pitch) as ObservableCollection<PlayerPosition>;
        }

        internal void SubmitTeam()
        {
            // Turn off the players whilst submitting
            players.Clear();
            NotifyPropertyChanged("Players");

            // Turn on loading
            IsLoading = true;
            IsSubmittedFailure = false;

            // Create a dictionary of locations from the pitch to pass to the api
            Dictionary<int, int> pitchDictionary;
            pitchDictionary = new Dictionary<int, int>();
            foreach (CurrentPitchLocationRecord cplr in pitch)
            {
                pitchDictionary.Add(cplr.PositionId, cplr.PlayerId);
            }

            // Create the api
            UserApi api = new UserApi();
            api.SubmitPitchCompleted += userApi_SubmitPitchCompleted;
            
            // Do the api call
            api.SubmitTeam(App.ViewModel.DbViewModel.CurrentUser.UserId, pitchDictionary);
        }

        #endregion


        #region Api Events

        void userApi_SubmitPitchCompleted(object sender, SubmitTeamEventArgs e)
        {
            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                IsLoading = false;
                IsSubmittedFailure = false;
            }
            else
            {
                IsLoading = false;
                IsSubmittedFailure = true;
            }

            if (Completed != null)
            {
                Completed(this, e);
            }
        }

        #endregion


        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
