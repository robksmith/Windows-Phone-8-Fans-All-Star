
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class CurrentPitchLocationRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private int _pitchLocationId;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int PitchLocationId
        {
            get { return _pitchLocationId; }
            set
            {
                NotifyPropertyChanging("PitchLocationId");
                _pitchLocationId = value;
                NotifyPropertyChanged("PitchLocationId");
            }
        }

        // player
        private int _playerId;

        [Column]
        public int PlayerId
        {
            get { return _playerId; }
            set
            {
                NotifyPropertyChanging("PlayerId");
                _playerId = value;
                NotifyPropertyChanged("PlayerId");
            }
        }

        // position
        private int _positionId;

        [Column]
        public int PositionId
        {
            get { return _positionId; }
            set
            {
                NotifyPropertyChanging("PositionId");
                _positionId = value;
                NotifyPropertyChanged("PositionId");
            }
        }



        // Assign handlers for the add and remove operations, respectively.
        public CurrentPitchLocationRecord()
        {
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