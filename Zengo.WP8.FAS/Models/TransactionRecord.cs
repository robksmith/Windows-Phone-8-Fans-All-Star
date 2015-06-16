
#region usings

using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Globalization;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class TransactionRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private string _transactionId;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "NVarChar(100) NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string TransactionId
        {
            get { return _transactionId; }
            set
            {
                if (_transactionId != value)
                {
                    NotifyPropertyChanging("TransactionId");
                    _transactionId = value;
                    NotifyPropertyChanged("TransactionId");
                }
            }
        }


        // PackageId
        private int _packageId;

        [Column]
        public int PackageId
        {
            get { return _packageId; }
            set
            {
                if (_packageId != value)
                {
                    NotifyPropertyChanging("PackageId");
                    _packageId = value;
                    NotifyPropertyChanged("PackageId");
                }
            }
        }


        // origin
        private string _origin;

        [Column]
        public string Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                {
                    NotifyPropertyChanging("Origin");
                    _origin = value;
                    NotifyPropertyChanged("Origin");
                }
            }
        }

        // amount
        private int _amount;

        [Column]
        public int Amount
        {
            get { return _amount; }
            set
            {
                if (_amount != value)
                {
                    NotifyPropertyChanging("Amount");
                    _amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }

        // TimeStamp
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

        public string FormattedAmount
        {
            get { return string.Format(new CultureInfo("en-GB"), "{0:c2}", (float)(Amount) / 100); }
            private set { }
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