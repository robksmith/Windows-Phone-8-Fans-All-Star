
#region usings

using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class VoteRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private string _voteId;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "NVarChar(100) NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string VoteId
        {
            get { return _voteId; }
            set
            {
                if (_voteId != value)
                {
                    NotifyPropertyChanging("VoteId");
                    _voteId = value;
                    NotifyPropertyChanged("VoteId");
                }
            }
        }

        // PlayerId
        private int _playerId;

        [Column]
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


        // PositionId
        private int _positionId;

        [Column]
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


        // Server Vote Created time
        private long _created;

        [Column]
        public long Created
        {
            get { return _created; }
            set
            {
                if (_created != value)
                {
                    NotifyPropertyChanging("Created");
                    _created = value;
                    NotifyPropertyChanged("Created");
                }
            }
        }


        // Server vote cast TimeStamp
        private long _timeStamp;

        [Column]
        public long TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                if (_timeStamp != value)
                {
                    NotifyPropertyChanging("TimeStamp");
                    _timeStamp = value;
                    NotifyPropertyChanged("TimeStamp");
                }
            }
        }


        // Local vote cast time timeStamp to time how long it takes
        private long _localCastTimeStamp;

        [Column]
        public long LocalCastTimeStamp
        {
            get { return _localCastTimeStamp; }
            set
            {
                if (_localCastTimeStamp != value)
                {
                    NotifyPropertyChanging("LocalCastTimeStamp");
                    _localCastTimeStamp = value;
                    NotifyPropertyChanged("LocalCastTimeStamp");
                }
            }
        }

        // This means submitted towards a pitch?
        private bool _submitted;

        [Column]
        public bool Submitted
        {
            get { return _submitted; }
            set
            {
                if (_submitted != value)
                {
                    NotifyPropertyChanging("Submitted");
                    _submitted = value;
                    NotifyPropertyChanged("Submitted");
                }
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