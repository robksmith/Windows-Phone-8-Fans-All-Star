
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    public class VotingHistoryRecord
    {
        public PlayerRecord PlayerRecord { get; set; }
        public string Position { get; set; }
        public DateTime VoteTime { get; set; }
    }

    [Table]
    public class PlayerRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private int _playerId;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int PlayerId
        {
            get { return _playerId; }
            set
            {
                if (_playerId != value)
                {
                    NotifyPropertyChanging("PlayerId");
                    _playerId = value;
                    NotifyPropertyChanged("PlayerId");
                }
            }
        }

        // first name
        private string _firstName;

        [Column]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    NotifyPropertyChanging("FirstName");
                    _firstName = value;
                    NotifyPropertyChanged("FirstName");
                }
            }
        }

        // last name
        private string _lastName;

        [Column]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    NotifyPropertyChanging("LastName");
                    _lastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        // culled
        private bool _isCulled;

        [Column]
        public bool IsCulled
        {
            get { return _isCulled; }
            set
            {
                if (_isCulled != value)
                {
                    NotifyPropertyChanging("IsCulled");
                    _isCulled = value;
                    NotifyPropertyChanged("IsCulled");
                }
            }
        }

        // deleted
        private bool _isDeleted;

        [Column]
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (_isDeleted != value)
                {
                    NotifyPropertyChanging("IsDeleted");
                    _isDeleted = value;
                    NotifyPropertyChanged("IsDeleted");
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

        // moving
        private int _moving;

        [Column]
        public int Moving
        {
            get { return _moving; }
            set
            {
                if (_moving != value)
                {
                    NotifyPropertyChanging("Moving");
                    _moving = value;
                    NotifyPropertyChanged("Moving");
                }
            }
        }


        // image
        private string _image;

        [Column]
        public string Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    NotifyPropertyChanging("Image");
                    _image = value;
                    NotifyPropertyChanged("Image");
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


        public string MovingImage
        {
            get 
            {
                if (Moving == -1)
                {
                    return "/Images/icn_pos_down.png";
                }
                else if (Moving == 1)
                {
                    return "/Images/icn_pos_up.png";
                }
                else
                {
                    return "/Images/icn_pos_nonmove.png";
                }
            }
            private set { }
        }

        public string IconImage
        {
            get
            {
                if (IsEurope)
                {
                    return "/Images/sta_player_eur.png";
                }
                else
                {
                    return "/Images/sta_player_row.png";
                }
            }
            private set { }
        }

        public string CulledMessageGrayedOutVisibility
        {
            get
            {
                if (IsCulled)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }
            }
            private set { }
        }

        public bool NotIsCulled
        {
            get { return !IsCulled; }
            private set { }
        }

        // Internal column for the associated Club ID value
        [Column]
        internal int _clubId;

        // Entity reference, to identify the Club "storage" table
        private EntityRef<ClubRecord> _club;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_club", ThisKey = "_clubId", OtherKey = "ClubId", IsForeignKey = true)]
        public ClubRecord Club
        {
            get { return _club.Entity; }
            set
            {
                NotifyPropertyChanging("Club");
                _club.Entity = value;

                if (value != null)
                {
                    _clubId = value.ClubId;
                }

                NotifyPropertyChanging("Club");
            }
        }


        
        // Internal column for the associated Country ID value
        [Column]
        internal int _countryId;

        // Entity reference, to identify the country "storage" table
        private EntityRef<CountryRecord> _country;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_country", ThisKey = "_countryId", OtherKey = "CountryId", IsForeignKey = true)]
        public CountryRecord Country
        {
            get { return _country.Entity; }
            set
            {
                NotifyPropertyChanging("Country");
                _country.Entity = value;

                if (value != null)
                {
                    _countryId = value.CountryId;
                }

                NotifyPropertyChanging("Country");
            }
        }


        // Internal column for the associated Country ID value
        [Column]
        internal int _zoneId;

        // Entity reference, to identify the country "storage" table
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