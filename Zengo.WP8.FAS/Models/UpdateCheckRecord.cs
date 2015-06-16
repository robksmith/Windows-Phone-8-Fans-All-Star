
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class UpdateCheckRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _updateCheckId;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int UpdateCheckId
        {
            get { return _updateCheckId; }
            set
            {
                NotifyPropertyChanging("UpdateCheckId");
                _updateCheckId = value;
                NotifyPropertyChanged("UpdateCheckId");
            }
        }

        // Define category name: private field, public property, and database column.
        private string _type;

        [Column]
        public string Type
        {
            get { return _type; }
            set
            {
                NotifyPropertyChanging("Type");
                _type = value;
                NotifyPropertyChanged("Type");
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


        // api count
        private int ?_apiCountNeeded;

        [Column]
        public int ?ApiCountNeeded
        {
            get { return _apiCountNeeded; }
            set
            {
                if (_apiCountNeeded != value)
                {
                    NotifyPropertyChanging("ApiCountNeeded");
                    _apiCountNeeded = value;
                    NotifyPropertyChanged("ApiCountNeeded");
                }
            }
        }

        // Has the database been updated successfully
        private bool _appliedSuccessfully;

        [Column]
        public bool AppliedSuccessfully
        {
            get { return _appliedSuccessfully; }
            set
            {
                if (_appliedSuccessfully != value)
                {
                    NotifyPropertyChanging("AppliedSuccessfully");
                    _appliedSuccessfully = value;
                    NotifyPropertyChanged("AppliedSuccessfully");
                }
            }
        }

        // date
        private DateTime _dateTime;

        [Column]
        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != value)
                {
                    NotifyPropertyChanging("DateTime");
                    _dateTime = value;
                    NotifyPropertyChanged("DateTime");
                }
            }
        }


        // Define the entity set for the collection side of the relationship.
        private EntitySet<ApiUpdateRecord> _updates;

        [Association(Storage = "_updates", OtherKey = "_updateCheckId", ThisKey = "UpdateCheckId")]
        public EntitySet<ApiUpdateRecord> Updates
        {
            get { return this._updates; }
            set { this._updates.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public UpdateCheckRecord()
        {
            _updates = new EntitySet<ApiUpdateRecord>(
                new Action<ApiUpdateRecord>(this.attach_Update),
                new Action<ApiUpdateRecord>(this.detach_Update)
                );
        }

        // Called during an add operation
        private void attach_Update(ApiUpdateRecord updateRecord)
        {
            NotifyPropertyChanging("UpdateRecord");
            updateRecord.UpdateCheck = this;
        }

        // Called during a remove operation
        private void detach_Update(ApiUpdateRecord updateRecord)
        {
            NotifyPropertyChanging("UpdateRecord");
            updateRecord.UpdateCheck = null;
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