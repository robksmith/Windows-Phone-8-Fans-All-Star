
#region usings

using Zengo.WP8.FAS.Helpers;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Windows;
using System.Linq;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class CountryRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _countryId;

        [Column(DbType = "INT NOT NULL", IsDbGenerated = false, IsPrimaryKey = true)]
        public int CountryId
        {
            get { return _countryId; }
            set
            {
                NotifyPropertyChanging("CountryId");
                _countryId = value;
                NotifyPropertyChanged("CountryId");
            }
        }

        // name
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

        // short name
        private string _shortName;

        [Column]
        public string ShortName
        {
            get { return _shortName; }
            set
            {
                NotifyPropertyChanging("ShortName");
                _shortName = value;
                NotifyPropertyChanged("ShortName");
            }
        }

        // telephone
        private string _telephone;

        [Column]
        public string Telephone
        {
            get { return _telephone; }
            set
            {
                NotifyPropertyChanging("Telephone");
                _telephone = value;
                NotifyPropertyChanged("Telephone");
            }
        }

        // short name
        private string _callCost;

        [Column]
        public string CallCost
        {
            get { return _callCost; }
            set
            {
                NotifyPropertyChanging("CallCost");
                _callCost = value;
                NotifyPropertyChanged("CallCost");
            }
        }

        // SMS
        private string _sms;

        [Column]
        public string Sms
        {
            get { return _sms; }
            set
            {
                NotifyPropertyChanging("Sms");
                _sms = value;
                NotifyPropertyChanged("Sms");
            }
        }

        // SMS cost
        private string _smsCost;

        [Column]
        public string SmsCost
        {
            get { return _smsCost; }
            set
            {
                NotifyPropertyChanging("SmsCost");
                _smsCost = value;
                NotifyPropertyChanged("SmsCost");
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

        // Selectable in the list
        private int _selectable;

        [Column]
        public int Selectable
        {
            get { return _selectable; }
            set
            {
                if (_selectable != value)
                {
                    NotifyPropertyChanging("Selectable");
                    _selectable = value;
                    NotifyPropertyChanged("Selectable");
                }
            }
        }

        private bool SelectableInSearch { get { return (_selectable == 0 || _selectable == 2); } }
        private bool SelectableInAccount { get { return (_selectable == 1 || _selectable == 2); } }


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

        /// <summary>
        /// Returns the local location of the http image
        /// </summary>
        public string ImageToRender
        {
            get 
            {
                string guidImage = ImageInGuidForm();
                string idImage = ImageInIdForm();

                if ( !string.IsNullOrEmpty( guidImage) )
                {
                    return guidImage;
                }
                if (!string.IsNullOrEmpty(idImage))
                {
                    return idImage;
                }

                return _image; 
            }
            private set { }
        }

        public string ImageInIdForm()
        {
            string idImage = "Images/Flags/" + CountryId.ToString() + ".png";
            if (FileInProject(idImage))
            {
                return "/" + idImage;
            }
            return string.Empty;
        }

        public string ImageInGuidForm()
        {
            if (!string.IsNullOrEmpty(_image) && Path.HasExtension(_image))
            {
                string filename = Path.GetFileNameWithoutExtension(_image);
                string extension = Path.GetExtension(_image);

                if (extension == ".gif")
                {
                    extension = ".png";
                }

                string potentialPath = "Images/Flags/" + filename + "@2x" + extension;

                if (FileInProject(potentialPath) && (extension == ".png" || extension == ".jpg" || extension == ".jpeg"))
                {
                    return "/" + potentialPath;
                }
            }

            return string.Empty;
        }

        // Because executing the Players.Count has a significant delay, we store it here
        private int _playerCount;

        [Column]
        public int PlayerCount
        {
            get { return _playerCount; }
            set
            {
                NotifyPropertyChanging("PlayerCount");
                _playerCount = value;
                NotifyPropertyChanged("PlayerCount");
            }
        }

        public string PlayersCountText
        {
            get
            {
                if (App.AppConstants.ApplyingUpdates) return string.Empty;

                int count = (from player in Players
                        where player.PlayerId != DatabaseHelper.DummyId
                        where player.IsDeleted == false
                        select player).Count();


                if (count == 1)
                {
                    return "1 player";
                }
                else
                {
                    return count + " players";
                }
            }
            private set
            {
            }
        }

        private static bool FileInProject(string file)
        {
            return Application.GetResourceStream(new Uri(file, UriKind.Relative)) != null;
        }


        // Internal column for the associated contininet ID value
        [Column]
        internal int _continentId;

        // Entity reference, to identify the ToDoCategory "storage" table
        private EntityRef<ContinentRecord> _continent;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_continent", ThisKey = "_continentId", OtherKey = "ContinentId", IsForeignKey = true)]
        public ContinentRecord Continent
        {
            get { return _continent.Entity; }
            set
            {
                NotifyPropertyChanging("Continent");
                _continent.Entity = value;

                if (value != null)
                {
                    _continentId = value.ContinentId;
                }

                NotifyPropertyChanging("Continent");
            }
        }
        


        // Define the entity set for the collection side of the relationship.
        private EntitySet<PlayerRecord> _players;

        [Association(Storage = "_players", OtherKey = "_countryId", ThisKey = "CountryId")]
        public EntitySet<PlayerRecord> Players
        {
            get { return this._players; }
            set { this._players.Assign(value); }
        }


        // Assign handlers for the add and remove operations, respectively.
        public CountryRecord()
        {
            _players = new EntitySet<PlayerRecord>(
                new Action<PlayerRecord>(this.attach_Player),
                new Action<PlayerRecord>(this.detach_Player)
                );
        }

        // Called during an add operation
        private void attach_Player(PlayerRecord player)
        {
            NotifyPropertyChanging("PlayerRecord");
            player.Country = this;
        }

        // Called during a remove operation
        private void detach_Player(PlayerRecord player)
        {
            NotifyPropertyChanging("PlayerRecord");
            player.Country = null;
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

        /// <summary>
        /// Pass
        /// </summary>
        /// <param name="forUserAccount"></param>
        /// <returns></returns>
        internal bool SelectableInUserAccount(bool forSearch)
        {
            if (forSearch)
            {
                return SelectableInSearch;
            }
            else
            {
                return SelectableInAccount;
            }
        }

        internal bool Show(bool onlyWithPlayers)
        {
            if (onlyWithPlayers)
            {
                return Players.Count >= 1;
            }
            else
            {
                return true;
            }
        }
    }

}