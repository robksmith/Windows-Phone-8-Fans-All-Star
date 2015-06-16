
#region usings

using Zengo.WP8.FAS.Helpers;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Globalization;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class PackageRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private static int freeId = 888888;
        public static int FreeId { get { return freeId; } private set { } }

        // Define ID: private field, public property, and database column.
        private int _packageId;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "INT NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int PackageId
        {
            get { return _packageId; }
            set
            {
                if (_packageId != value)
                {
                    NotifyPropertyChanging("PackagesId");
                    _packageId = value;
                    NotifyPropertyChanged("PackagesId");
                }
            }
        }

        // Number of votes
        private int _votes;

        [Column]
        public int Votes
        {
            get { return _votes; }
            set
            {
                if (_votes != value)
                {
                    NotifyPropertyChanging("Votes");
                    _votes = value;
                    NotifyPropertyChanged("Votes");
                }
            }
        }


        // Price
        private int _price;

        [Column]
        public int Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    NotifyPropertyChanging("Price");
                    _price = value;
                    NotifyPropertyChanged("Price");
                }
            }
        }


        // Name
        private string _name;

        [Column]
        public string Name
        {
            get 
            {
                return _name; 
            }
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


        // Bid
        private string _bid;

        [Column]
        public string Bid
        {
            get { return _bid; }
            set
            {
                if (_bid != value)
                {
                    NotifyPropertyChanging("Bid");
                    _bid = value;
                    NotifyPropertyChanged("Bid");
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


        public string FormattedPrice
        {
            get 
            {
                if (Price <= 0) 
                    return "Free";
                else
                    return string.Format(new CultureInfo("en-us"), "{0:c2}", (float)(Price) / 100);
                    //return string.Format("{0:F}", (float)(Price) / 100); 
            }
            private set { }
        }

        public string Image
        {
            get 
            {
                if ( _votes <=1)
                {
                    return "/Images/icn_menu_star_0.png";
                }
                else if (_votes <= 30)
                {
                    return "/Images/icn_menu_star_1.png";
                }
                else if (_votes <= 60)
                {
                    return "/Images/icn_menu_star_2.png";
                }
                else if (_votes <= 90)
                {
                    return "/Images/icn_menu_star_3.png";
                }
                else 
                {
                    return "/Images/icn_menu_star_4.png";
                }
            }
            private set { }
        }


        public string BuyText
        {
            get
            {
                if (_packageId == FreeId)
                {
                    return "Use my";
                }
                return "Buy";
            }
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