
#region usings

using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    [Table]
    public class UserRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        private string _userId;

        [Column(IsPrimaryKey = true, IsDbGenerated = false, DbType = "NVarChar(100) NOT NULL", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public string UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    NotifyPropertyChanging("UserId");
                    _userId = value;
                    NotifyPropertyChanged("UserId");
                }
            }
        }


        // nid
        private string _nid;

        [Column]
        public string Nid
        {
            get { return _nid; }
            set
            {
                if (_nid != value)
                {
                    NotifyPropertyChanging("Nid");
                    _nid = value;
                    NotifyPropertyChanged("Nid");
                }
            }
        }


        // first name
        private string _firstName;

        [Column]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    NotifyPropertyChanging("FirstName");
                    _firstName = value;
                    NotifyPropertyChanged("FirstName");
                }
            }
        }

        // last name
        private string _lastName;

        [Column]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    NotifyPropertyChanging("LastName");
                    _lastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        // IsValidated
        private bool _isValidated;

        [Column]
        public bool IsValidated
        {
            get { return _isValidated; }
            set
            {
                if (_isValidated != value)
                {
                    NotifyPropertyChanging("IsValidated");
                    _isValidated = value;
                    NotifyPropertyChanged("IsValidated");
                }
            }
        }


        // Submitted Answers - set locally
        private bool _submittedAnswers;

        [Column]
        public bool SubmittedAnswers
        {
            get { return _submittedAnswers; }
            set
            {
                if (_submittedAnswers != value)
                {
                    NotifyPropertyChanging("SubmittedAnswers");
                    _submittedAnswers = value;
                    NotifyPropertyChanged("SubmittedAnswers");
                }
            }
        }

        // Is entry to the competition available to them - from server
        private bool _entryAvailable;

        [Column]
        public bool EntryAvailable
        {
            get { return _entryAvailable; }
            set
            {
                if (_entryAvailable != value)
                {
                    NotifyPropertyChanging("EntryAvailable");
                    _entryAvailable = value;
                    NotifyPropertyChanged("EntryAvailable");
                }
            }
        }

        public bool ShowFreeQuestion { get { return !SubmittedAnswers && EntryAvailable; } }

        // fav 1
        private int _fav1;

        [Column]
        public int Fav1
        {
            get { return _fav1; }
            set
            {
                if (_fav1 != value)
                {
                    NotifyPropertyChanging("Fav1");
                    _fav1 = value;
                    NotifyPropertyChanged("Fav1");
                }
            }
        }


        // fav 2
        private int _fav2;

        [Column]
        public int Fav2
        {
            get { return _fav2; }
            set
            {
                if (_fav2 != value)
                {
                    NotifyPropertyChanging("Fav2");
                    _fav2 = value;
                    NotifyPropertyChanged("Fav2");
                }
            }
        }


        // my country
        private int _myCountry;

        [Column]
        public int MyCountry
        {
            get { return _myCountry; }
            set
            {
                if (_myCountry != value)
                {
                    NotifyPropertyChanging("MyCountry");
                    _myCountry = value;
                    NotifyPropertyChanged("MyCountry");
                }
            }
        }


        // TeamName
        private string _teamName;

        [Column]
        public string TeamName
        {
            get { return _teamName; }
            set
            {
                if (_teamName != value)
                {
                    NotifyPropertyChanging("TeamName");
                    _teamName = value;
                    NotifyPropertyChanged("TeamName");
                }
            }
        }

        // Email
        private string _email;

        [Column]
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    NotifyPropertyChanging("Email");
                    _email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }

        // Mobile
        private string _mobile;

        [Column]
        public string Mobile
        {
            get { return _mobile; }
            set
            {
                if (_mobile != value)
                {
                    NotifyPropertyChanging("Mobile");
                    _mobile = value;
                    NotifyPropertyChanged("Mobile");
                }
            }
        }


        private int _totalVotesPurchased;
        [Column]
        public int TotalVotesPurchased
        {
            get { return _totalVotesPurchased; }
            set
            {
                if (_totalVotesPurchased != value)
                {
                    NotifyPropertyChanging("TotalVotesPurchased");
                    _totalVotesPurchased = value;
                    NotifyPropertyChanged("TotalVotesPurchased");
                }
            }
        }


        // the last time we retrieved the AVAILABLE  vote history
        private long _lastAvailableVoteHistoryRetrievalTime;

        [Column]
        public long LastAvailableVoteHistoryRetrievalTime
        {
            get { return _lastAvailableVoteHistoryRetrievalTime; }
            set
            {
                if (_lastAvailableVoteHistoryRetrievalTime != value)
                {
                    NotifyPropertyChanging("LastAvailableVoteHistoryRetrievalTime");
                    _lastAvailableVoteHistoryRetrievalTime = value;
                    NotifyPropertyChanged("LastAvailableVoteHistoryRetrievalTime");
                }
            }
        }

        // the last time we retrieved the CAST vote history
        private long _lastCastVoteHistoryRetrievalTime;

        [Column]
        public long LastCastVoteHistoryRetrievalTime
        {
            get { return _lastCastVoteHistoryRetrievalTime; }
            set
            {
                if (_lastCastVoteHistoryRetrievalTime != value)
                {
                    NotifyPropertyChanging("LastCastVoteHistoryRetrievalTime");
                    _lastCastVoteHistoryRetrievalTime = value;
                    NotifyPropertyChanged("LastCastVoteHistoryRetrievalTime");
                }
            }
        }

        // The last client time we returned the response from a vote
        private DateTime _lastLocalCastTime;

        [Column]
        public DateTime LastLocalCastTime
        {
            get { return _lastLocalCastTime; }
            set
            {
                if (_lastLocalCastTime != value)
                {
                    NotifyPropertyChanging("LastLocalCastTime");
                    _lastLocalCastTime = value;
                    NotifyPropertyChanged("LastLocalCastTime");
                }
            }
        }


        // the last time we retrieved the squad team history
        private long _lastSquadHistoryRetrievalTime;

        [Column]
        public long LastSquadHistoryRetrievalTime
        {
            get { return _lastSquadHistoryRetrievalTime; }
            set
            {
                if (_lastSquadHistoryRetrievalTime != value)
                {
                    NotifyPropertyChanging("LastSquadHistoryRetrievalTime");
                    _lastSquadHistoryRetrievalTime = value;
                    NotifyPropertyChanged("LastSquadHistoryRetrievalTime");
                }
            }
        }

        public string EmailText { get { return string.Format("{0}", Email); } private set { } }
        public string NidText { get { return string.Format("SMS / Phone ID: {0}", Nid); } private set { } }
        public string VotesText
        {
            get
            {
                return string.Format("votes available {0} : votes cast {1}", App.ViewModel.DbViewModel.AvailableVotesCount(), App.ViewModel.DbViewModel.TotalVotesPurchased() - App.ViewModel.DbViewModel.AvailableVotesCount());
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