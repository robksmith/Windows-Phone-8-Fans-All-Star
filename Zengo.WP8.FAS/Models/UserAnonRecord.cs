
#region usings

using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class UserAnonRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private string _anonUserId;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public string AnonUserId
        {
            get { return _anonUserId; }
            set
            {
                if (_anonUserId != value)
                {
                    NotifyPropertyChanging("AnonUserId");
                    _anonUserId = value;
                    NotifyPropertyChanged("AnonUserId");
                }
            }
        }


        // Language
        private string _language;

        [Column]
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    NotifyPropertyChanging("Language");
                    _language = value;
                    NotifyPropertyChanged("Language");
                }
            }
        }


        // the last time we retrieved the moving values
        private long _lastMovingRetrievalTime;

        [Column]
        public long LastMovingRetrievalTime
        {
            get { return _lastMovingRetrievalTime; }
            set
            {
                if (_lastMovingRetrievalTime != value)
                {
                    NotifyPropertyChanging("LastMovingRetrievalTime");
                    _lastMovingRetrievalTime = value;
                    NotifyPropertyChanged("LastMovingRetrievalTime");
                }
            }
        }

        // the last time we retrieved the update history
        private long _lastUpdateRetrievalTime;

        [Column]
        public long LastUpdateRetrievalTime
        {
            get { return _lastUpdateRetrievalTime; }
            set
            {
                if (_lastUpdateRetrievalTime != value)
                {
                    NotifyPropertyChanging("LastUpdateRetrievalTime");
                    _lastUpdateRetrievalTime = value;
                    NotifyPropertyChanged("LastUpdateRetrievalTime");
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