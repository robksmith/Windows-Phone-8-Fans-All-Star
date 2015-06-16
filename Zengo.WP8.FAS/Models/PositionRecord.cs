
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class PositionRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private int _positionId;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int PositionId
        {
            get { return _positionId; }
            set
            {
                if (_positionId != value)
                {
                    NotifyPropertyChanging("PositionId");
                    _positionId = value;
                    NotifyPropertyChanged("PositionId");
                }
            }
        }

        // key
        private string _key;

        [Column]
        public string Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    NotifyPropertyChanging("Key");
                    _key = value;
                    NotifyPropertyChanged("Key");
                }
            }
        }

        // name
        private string _name;

        [Column]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
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

        // Europe
        private bool _isEurope;

        [Column]
        public bool IsEurope
        {
            get { return _isEurope; }
            set
            {
                if (_isEurope != value)
                {
                    NotifyPropertyChanging("IsEurope");
                    _isEurope = value;
                    NotifyPropertyChanged("IsEurope");
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


        // Internal column for the associated Club ID value
        [Column]
        internal int _zoneId;

        // Entity reference, to identify the Club "storage" table
        private EntityRef<ZoneRecord> _zone;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_zone", ThisKey = "_zoneId", OtherKey = "ZoneId", IsForeignKey = true)]
        public ZoneRecord Zone
        {
            get { return _zone.Entity; }
            set
            {
                NotifyPropertyChanging("Zone");
                _zone.Entity = value;

                if (value != null)
                {
                    _zoneId = value.ZoneId;
                }

                NotifyPropertyChanging("Zone");
            }
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