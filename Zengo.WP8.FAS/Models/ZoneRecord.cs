
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
    public class ZoneRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _zoneId;

        [Column(DbType = "INT NOT NULL", IsDbGenerated = false, IsPrimaryKey = true)]
        public int ZoneId
        {
            get { return _zoneId; }
            set
            {
                NotifyPropertyChanging("ZoneId");
                _zoneId = value;
                NotifyPropertyChanged("ZoneId");
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



        // group
        private string _group;

        [Column]
        public string Group
        {
            get { return _group; }
            set
            {
                if (_group != value)
                {
                    NotifyPropertyChanging("Group");
                    _group = value;
                    NotifyPropertyChanged("Group");
                }
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

        // Define the entity set for the collection side of the relationship.
        private EntitySet<PlayerRecord> _players;

        [Association(Storage = "_players", OtherKey = "_zoneId", ThisKey = "ZoneId")]
        public EntitySet<PlayerRecord> Players
        {
            get { return this._players; }
            set { this._players.Assign(value); }
        }



        // Define the entity set for the collection side of the relationship.
        private EntitySet<PositionRecord> _positions;

        [Association(Storage = "_positions", OtherKey = "_zoneId", ThisKey = "ZoneId")]
        public EntitySet<PositionRecord> Positions
        {
            get { return this._positions; }
            set { this._positions.Assign(value); }
        }




        // Assign handlers for the add and remove operations, respectively.
        public ZoneRecord()
        {
            _players = new EntitySet<PlayerRecord>(
                new Action<PlayerRecord>(this.attach_Player),
                new Action<PlayerRecord>(this.detach_Player)
                );

            _positions = new EntitySet<PositionRecord>(
                new Action<PositionRecord>(this.attach_Player),
                new Action<PositionRecord>(this.detach_Player)
                );
        }

        // Called during an add operation
        private void attach_Player(PlayerRecord player)
        {
            NotifyPropertyChanging("PlayerRecord");
            player.Zone = this;
        }

        // Called during a remove operation
        private void detach_Player(PlayerRecord player)
        {
            NotifyPropertyChanging("PlayerRecord");
            player.Zone = null;
        }

        // Called during an add operation
        private void attach_Player(PositionRecord position)
        {
            NotifyPropertyChanging("PositionRecord");
            position.Zone = this;
        }

        // Called during a remove operation
        private void detach_Player(PositionRecord position)
        {
            NotifyPropertyChanging("PositionRecord");
            position.Zone = null;
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