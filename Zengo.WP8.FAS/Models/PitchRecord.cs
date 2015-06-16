
#region usings

using Zengo.WP8.FAS.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Globalization;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class PitchRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private int _pitchId;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int PitchId
        {
            get { return _pitchId; }
            set
            {
                NotifyPropertyChanging("PitchId");
                _pitchId = value;
                NotifyPropertyChanged("PitchId");
            }
        }

        // _name
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

        // status
        private string _status;

        [Column]
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    NotifyPropertyChanging("Status");
                    _status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }


        // the submitted timestamp
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

        public DateTime Date { get { return DateTimeHelper.DateTimeFromUnixTimestampMillis(TimeStamp); } private set {} }

        // The formatted date time
        public string FormattedDate
        {
            get
            {
                if (TimeStamp <= 0)
                    return "Unknown";
                else
                    return string.Format(new CultureInfo("en-GB"), "{0:dd MMM yyyy} at {1:HH:mm:ss}", Date, Date);
            }
            private set { }
        }

        //// date
        //private DateTime _dateTime;

        //[Column]
        //public DateTime DateTime
        //{
        //    get { return _dateTime; }
        //    set
        //    {
        //        if (_dateTime != value)
        //        {
        //            NotifyPropertyChanging("DateTime");
        //            _dateTime = value;
        //            NotifyPropertyChanged("DateTime");
        //        }
        //    }
        //}


        // Define the entity set for the collection side of the relationship.
        private EntitySet<PitchLocationRecord> _pitch;

        [Association(Storage = "_pitch", OtherKey = "_pitchId", ThisKey = "PitchId")]
        public EntitySet<PitchLocationRecord> Pitch
        {
            get { return this._pitch; }
            set { this._pitch.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public PitchRecord()
        {
            _pitch = new EntitySet<PitchLocationRecord>(
                new Action<PitchLocationRecord>(this.attach_Update),
                new Action<PitchLocationRecord>(this.detach_Update)
                );
        }

        internal PitchLocationRecord FindLocation(PositionRecord position)
        {
            return (from p in Pitch
                     where p.PositionId == position.PositionId
                     select p).FirstOrDefault();
        }


        // Called during an add operation
        private void attach_Update(PitchLocationRecord pitchLocationRecord)
        {
            NotifyPropertyChanging("PitchRecord");
            pitchLocationRecord.Pitch = this;
        }

        // Called during a remove operation
        private void detach_Update(PitchLocationRecord pitchLocationRecord)
        {
            NotifyPropertyChanging("PitchRecord");
            pitchLocationRecord.Pitch = null;
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