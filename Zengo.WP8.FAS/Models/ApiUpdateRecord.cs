
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class ApiUpdateRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _updateId;

        [Column(DbType = "INT NOT NULL IDENTITY", IsDbGenerated = true, IsPrimaryKey = true)]
        public int UpdateId
        {
            get { return _updateId; }
            set
            {
                NotifyPropertyChanging("UpdateId");
                _updateId = value;
                NotifyPropertyChanged("UpdateId");
            }
        }

        // name
        private string _apiName;

        [Column]
        public string ApiName
        {
            get { return _apiName; }
            set
            {
                NotifyPropertyChanging("ApiName");
                _apiName = value;
                NotifyPropertyChanged("ApiName");
            }
        }


        // count
        private int _recordCount;

        [Column]
        public int RecordCount
        {
            get { return _recordCount; }
            set
            {
                NotifyPropertyChanging("RecordCount");
                _recordCount = value;
                NotifyPropertyChanged("RecordCount");
            }
        }

        // success
        private bool _isSuccess;

        [Column]
        public bool IsSuccess
        {
            get { return _isSuccess; }
            set
            {
                if (_isSuccess != value)
                {
                    NotifyPropertyChanging("IsSuccess");
                    _isSuccess = value;
                    NotifyPropertyChanged("IsSuccess");
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


        // Internal column for the associated contininet ID value
        [Column]
        internal int _updateCheckId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<UpdateCheckRecord> _updateCheck;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_updateCheck", ThisKey = "_updateCheckId", OtherKey = "UpdateCheckId", IsForeignKey = true)]
        public UpdateCheckRecord UpdateCheck
        {
            get { return _updateCheck.Entity; }
            set
            {
                NotifyPropertyChanging("UpdateCheck");
                _updateCheck.Entity = value;

                if (value != null)
                {
                    _updateCheckId = value.UpdateCheckId;
                }

                NotifyPropertyChanging("UpdateCheck");
            }
        }
        


        //// Define the entity set for the collection side of the relationship.
        //private EntitySet<PlayerRecord> _players;

        //[Association(Storage = "_players", OtherKey = "_countryId", ThisKey = "CountryId")]
        //public EntitySet<PlayerRecord> Players
        //{
        //    get { return this._players; }
        //    set { this._players.Assign(value); }
        //}


        // Assign handlers for the add and remove operations, respectively.
        public ApiUpdateRecord()
        {
            //_players = new EntitySet<PlayerRecord>(
            //    new Action<PlayerRecord>(this.attach_Player),
            //    new Action<PlayerRecord>(this.detach_Player)
            //    );
        }

        //// Called during an add operation
        //private void attach_Player(UpdateCheckRecord player)
        //{
        //    NotifyPropertyChanging("PlayerRecord");
        //    player.Country = this;
        //}

        //// Called during a remove operation
        //private void detach_Player(UpdateCheckRecord player)
        //{
        //    NotifyPropertyChanging("PlayerRecord");
        //    player.Country = null;
        //}





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