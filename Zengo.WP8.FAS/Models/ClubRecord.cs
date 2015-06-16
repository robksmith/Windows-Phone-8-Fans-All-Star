
#region usings

using Zengo.WP8.FAS.Helpers;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class ClubRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _clubId;

        [Column(DbType = "INT NOT NULL", IsDbGenerated = false, IsPrimaryKey = true)]
        public int ClubId
        {
            get { return _clubId; }
            set
            {
                NotifyPropertyChanging("ClubId");
                _clubId = value;
                NotifyPropertyChanged("ClubId");
            }
        }

        // Define category name: private field, public property, and database column.
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                NotifyPropertyChanging("Name");
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        // Image
        private string _image;

        [Column]
        public string Image
        {
            get { return _image; }
            set
            {
                NotifyPropertyChanging("Image");
                _image = value;
                NotifyPropertyChanged("Image");
            }
        }

        // last modified
        private DateTime _lastModified;

        [Column]
        public DateTime LastModified
        {
            get { return _lastModified; }
            set
            {
                if (_lastModified != value)
                {
                    NotifyPropertyChanging("LastModified");
                    _lastModified = value;
                    NotifyPropertyChanged("LastModified");
                }
            }
        }


        private string _country;

        [Column]
        public string Country
        {
            get { return _country; }
            set
            {
                NotifyPropertyChanging("Country");
                _country = value;
                NotifyPropertyChanged("Country");
            }
        }


        public string PlayersCountText
        {
            get 
            {
                if (App.AppConstants.ApplyingUpdates) return string.Empty;

                int count = (from player in Players
                             where player.PlayerId != DatabaseHelper.DummyId
                             where player.IsDeleted == false
                             select player).Count();

                if (count == 1)
                {
                    return "1 player";
                }
                else
                {
                    return count + " players";
                }
            }
            private set
            {
            }
        }



        // Internal column for the associated Club ID value
        [Column]
        internal int _leagueId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<LeagueRecord> _league;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_league", ThisKey = "_leagueId", OtherKey = "LeagueId", IsForeignKey = true)]
        public LeagueRecord League
        {
            get { return _league.Entity; }
            set
            {
                NotifyPropertyChanging("League");
                _league.Entity = value;

                if (value != null)
                {
                    _leagueId = value.LeagueId;
                }

                NotifyPropertyChanging("League");
            }
        }

        // Because executing the Players.Count has a significant delay, we store it here
        private int _playerCount;

        [Column]
        public int PlayerCount
        {
            get { return _playerCount; }
            set
            {
                NotifyPropertyChanging("PlayerCount");
                _playerCount = value;
                NotifyPropertyChanged("PlayerCount");
            }
        }


        // Define the entity set for the collection side of the relationship.
        private EntitySet<PlayerRecord> _players;

        [Association(Storage = "_players", OtherKey = "_clubId", ThisKey = "ClubId")]
        public EntitySet<PlayerRecord> Players
        {
            get { return this._players; }
            set { this._players.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public ClubRecord()
        {
            _players = new EntitySet<PlayerRecord>(
                new Action<PlayerRecord>(this.attach_Player),
                new Action<PlayerRecord>(this.detach_Player)
                );
        }

        // Called during an add operation
        private void attach_Player(PlayerRecord player)
        {
            NotifyPropertyChanging("PlayerRecord");
            player.Club = this;
        }

        // Called during a remove operation
        private void detach_Player(PlayerRecord player)
        {
            NotifyPropertyChanging("PlayerRecord");
            player.Club = null;
        }





        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (App.AppConstants.ApplyingUpdates) return;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

    }

}