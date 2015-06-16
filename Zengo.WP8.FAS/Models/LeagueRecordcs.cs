
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class LeagueRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _leagueId;

        [Column(DbType = "INT NOT NULL", IsDbGenerated = false, IsPrimaryKey = true)]
        public int LeagueId
        {
            get { return _leagueId; }
            set
            {
                NotifyPropertyChanging("LeagueId");
                _leagueId = value;
                NotifyPropertyChanged("LeagueId");
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


        // Define the entity set for the collection side of the relationship.
        private EntitySet<ClubRecord> _clubs;

        [Association(Storage = "_clubs", OtherKey = "_leagueId", ThisKey = "LeagueId")]
        public EntitySet<ClubRecord> Clubs
        {
            get { return this._clubs; }
            set { this._clubs.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public LeagueRecord()
        {
            _clubs = new EntitySet<ClubRecord>(
                new Action<ClubRecord>(this.attach_Club),
                new Action<ClubRecord>(this.detach_Club)
                );
        }

        // Called during an add operation
        private void attach_Club(ClubRecord club)
        {
            NotifyPropertyChanging("ClubRecord");
            club.League = this;
        }

        // Called during a remove operation
        private void detach_Club(ClubRecord club)
        {
            NotifyPropertyChanging("ClubRecord");
            club.League = null;
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