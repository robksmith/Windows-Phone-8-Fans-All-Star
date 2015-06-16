
#region Usings

using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

#endregion

namespace Zengo.WP8.FAS.ViewModels
{
    public class PlayerStatsViewModel : INotifyPropertyChanged
    {
        #region Fields

        private bool isLoading;
        private ObservableCollection<PlayerStatsResponse.Stat> stats;

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

        public ObservableCollection<PlayerStatsResponse.Stat> Stats
        {
            get { return stats; }
            set
            {
                if (stats != value)
                {
                    stats = value;
                    NotifyPropertyChanged("Stats");
                }
            }
        }
        #endregion


        #region Constructors

        public PlayerStatsViewModel()
        {
            stats = new ObservableCollection<PlayerStatsResponse.Stat>();

            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 1", Value = "Value 1" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 2", Value = "Value 2" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 3", Value = "Value 3" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 4", Value = "Value 4" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 5", Value = "Value 5" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 6", Value = "Value 6" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 7", Value = "Value 7" });
            stats.Add(new PlayerStatsResponse.Stat() { Name = "Stat 8", Value = "Value 8" });
        }

        public PlayerStatsViewModel(int playerId)
        {
            IsLoading = true;

            stats = new ObservableCollection<PlayerStatsResponse.Stat>();

            UserApi api = new UserApi();
            api.PlayerStatsCompleted += api_PlayerStatsCompleted;
            api.PlayerStartsQuery(playerId);
        }

        #endregion


        #region Api Events

        /// <summary>
        /// The stats api has returned
        /// </summary>
        void api_PlayerStatsCompleted(object sender, PlayerStatsEventArgs e)
        {
            stats.Clear();

            if (e.ServerResponse != null)
            {
                foreach (PlayerStatsResponse.Stat stat in e.ServerResponse.Response.Stats.Vals)
                {
                    stats.Add(stat);
                }
            }
            else
            {
                stats.Add(new PlayerStatsResponse.Stat() { Name = "No data" });
            }

            NotifyPropertyChanged("Stats");

            IsLoading = false;
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
