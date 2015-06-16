
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class ContinentRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _continentId;

        [Column(DbType = "INT NOT NULL", IsDbGenerated = false, IsPrimaryKey = true)]
        public int ContinentId
        {
            get { return _continentId; }
            set
            {
                NotifyPropertyChanging("ContinentId");
                _continentId = value;
                NotifyPropertyChanged("ContinentId");
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
        private EntitySet<CountryRecord> _countries;

        [Association(Storage = "_countries", OtherKey = "_continentId", ThisKey = "ContinentId")]
        public EntitySet<CountryRecord> Countries
        {
            get { return this._countries; }
            set { this._countries.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public ContinentRecord()
        {
            _countries = new EntitySet<CountryRecord>(
                new Action<CountryRecord>(this.attach_Country),
                new Action<CountryRecord>(this.detach_Country)
                );
        }

        // Called during an add operation
        private void attach_Country(CountryRecord country)
        {
            NotifyPropertyChanging("ClubRecord");
            country.Continent = this;
        }

        // Called during a remove operation
        private void detach_Country(CountryRecord country)
        {
            NotifyPropertyChanging("ClubRecord");
            country.Continent = null;
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