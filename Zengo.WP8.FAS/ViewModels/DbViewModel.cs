
#region Usings

using Microsoft.Phone.Controls;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Controls.ListItems;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;

#endregion

namespace Zengo.WP8.FAS.ViewModels
{
    public class VotePositionRecord
    {
        public VoteRecord VotePecord { get; set; }
        public PlayerRecord PlayerRecord { get; set; }
        public DateTime VoteTime { get; set; }

        public PlayerIconControl Icon { get; set; }

        public PositionRecord Position { get; set; }
    }

    public class DbViewModel : INotifyPropertyChanged
    {
        #region Fields

        //Dictionary<int, int> positionsDictionary;
        //Dictionary<int, int> playersDictionary;
        //Dictionary<int, int> pitchDictionary;

        #endregion


        #region Constructors

        public DbViewModel()
        {
            //positionsDictionary = new Dictionary<int, int>();
            //playersDictionary = new Dictionary<int, int>();
            //pitchDictionary = new Dictionary<int, int>();
        }

        #endregion


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


        #region Properties

        // The logged in user
        public UserRecord CurrentUser { get; set; }

        public UserAnonRecord CurrentAnonUser { get; set; }

        // All votes
        private ObservableCollection<VoteRecord> _allVotes;
        public ObservableCollection<VoteRecord> AllVotes
        {
            get { return _allVotes; }
            set
            {
                _allVotes = value;
                NotifyPropertyChanged("AllVotes");
            }
        }



        // All current pitch locations
        private ObservableCollection<CurrentPitchLocationRecord> _allCurrentPitchLocations;
        public ObservableCollection<CurrentPitchLocationRecord> AllCurrentPitchLocations
        {
            get { return _allCurrentPitchLocations; }
            set
            {
                _allCurrentPitchLocations = value;
                NotifyPropertyChanged("AllCurrentPitchLocations");
            }
        }



        // All pitches
        private ObservableCollection<PitchRecord> _allPitches;
        public ObservableCollection<PitchRecord> AllPitches
        {
            get { return _allPitches; }
            set
            {
                _allPitches = value;
                NotifyPropertyChanged("AllPitches");
            }
        }

        // All pitch locations
        private ObservableCollection<PitchLocationRecord> _allPitchLocations;
        public ObservableCollection<PitchLocationRecord> AllPitchLocations
        {
            get { return _allPitchLocations; }
            set
            {
                _allPitchLocations = value;
                NotifyPropertyChanged("AllPitchLocations");
            }
        }





        // All transactions
        private ObservableCollection<TransactionRecord> _allTransaction;
        public ObservableCollection<TransactionRecord> AllTransactions
        {
            get { return _allTransaction; }
            set
            {
                _allTransaction = value;
                NotifyPropertyChanged("AllTransaction");
            }
        }

        // All positions
        private ObservableCollection<PositionRecord> _allPositions;
        public ObservableCollection<PositionRecord> AllPositions
        {
            get { return _allPositions; }
            set
            {
                _allPositions = value;
                NotifyPropertyChanged("AllPositions");
            }
        }

        // All players
        private ObservableCollection<PlayerRecord> _allPlayers;
        public ObservableCollection<PlayerRecord> AllPlayers
        {
            get { return _allPlayers; }
            set
            {
                _allPlayers = value;
                NotifyPropertyChanged("AllPlayers");
            }
        }

        private ObservableCollection<ClubRecord> _allClubs;
        public ObservableCollection<ClubRecord> AllClubs
        {
            get { return _allClubs; }
            set
            {
                _allClubs = value;
                NotifyPropertyChanged("AllClubs");
            }
        }


        private ObservableCollection<ZoneRecord> _allZones;
        public ObservableCollection<ZoneRecord> AllZones
        {
            get { return _allZones; }
            set
            {
                _allZones = value;
                NotifyPropertyChanged("AllZones");
            }
        }

        private ObservableCollection<CountryRecord> _allCountries;
        public ObservableCollection<CountryRecord> AllCountries
        {
            get { return _allCountries; }
            set
            {
                _allCountries = value;
                NotifyPropertyChanged("AllCountries");
            }
        }

        private ObservableCollection<ContinentRecord> _allContinents;
        public ObservableCollection<ContinentRecord> AllContinents
        {
            get { return _allContinents; }
            set
            {
                _allContinents = value;
                NotifyPropertyChanged("AllContinents");
            }
        }

        private ObservableCollection<PackageRecord> _allPackages;
        public ObservableCollection<PackageRecord> AllPackages
        {
            get { return _allPackages; }
            set
            {
                _allPackages = value;
                NotifyPropertyChanged("AllPackages");
            }
        }

        private ObservableCollection<LeagueRecord> _allLeagues;
        public ObservableCollection<LeagueRecord> AllLeagues
        {
            get { return _allLeagues; }
            set
            {
                _allLeagues = value;
                NotifyPropertyChanged("AllLeagues");
            }
        }

        private ObservableCollection<ApiUpdateRecord> _allUpdates;
        public ObservableCollection<ApiUpdateRecord> AllApiUpdates
        {
            get { return _allUpdates; }
            set
            {
                _allUpdates = value;
                NotifyPropertyChanged("AllUpdates");
            }
        }

        private ObservableCollection<UpdateCheckRecord> _allUpdateChecks;
        public ObservableCollection<UpdateCheckRecord> AllUpdateChecks
        {
            get { return _allUpdateChecks; }
            set
            {
                _allUpdateChecks = value;
                NotifyPropertyChanged("AllUpdateChecks");
            }
        }
        #endregion


        #region Load

        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {
            // users query
            CurrentUser = null;
            int userCount = (from UserRecord user in DatabaseHelper.FasDataContext.Users select user).Count();
            if (userCount == 1)
            {
                CurrentUser = (from UserRecord user in DatabaseHelper.FasDataContext.Users select user).First();
            }

            // anon user
            int userAnonCount = (from UserAnonRecord user in DatabaseHelper.FasDataContext.AnonUsers select user).Count();
            if (userAnonCount >= 1)
            {
                CurrentAnonUser = (from UserAnonRecord user in DatabaseHelper.FasDataContext.AnonUsers select user).First();
            }
            else
            {
                CurrentAnonUser = new UserAnonRecord() { Language = "en-gb" };
                DatabaseHelper.FasDataContext.AnonUsers.InsertOnSubmit(CurrentAnonUser);
            }

            // current pitch locations
            if (true)
            {
                var currentPitchLocationsInDB = from CurrentPitchLocationRecord cpl in DatabaseHelper.FasDataContext.CurrentPitchLocations select cpl;
                AllCurrentPitchLocations = new ObservableCollection<CurrentPitchLocationRecord>(currentPitchLocationsInDB);
            }

            // historical pitches
            if (true)
            {
                var pitchesInDB = from PitchRecord pitch in DatabaseHelper.FasDataContext.Pitches select pitch;
                AllPitches = new ObservableCollection<PitchRecord>(pitchesInDB);

                var pitchLocationsInDB = from PitchLocationRecord pitchLocation in DatabaseHelper.FasDataContext.PitchLocation select pitchLocation;
                AllPitchLocations = new ObservableCollection<PitchLocationRecord>(pitchLocationsInDB);
            }


            var votesInDB = from VoteRecord vote in DatabaseHelper.FasDataContext.Votes select vote;
            AllVotes = new ObservableCollection<VoteRecord>(votesInDB);

            var transactionsInDB = from TransactionRecord transaction in DatabaseHelper.FasDataContext.Transactions select transaction;
            AllTransactions = new ObservableCollection<TransactionRecord>(transactionsInDB);


            var apiUpdatesInDB = from ApiUpdateRecord apiUpdate in DatabaseHelper.FasDataContext.ApiUpdates select apiUpdate;
            AllApiUpdates = new ObservableCollection<ApiUpdateRecord>(apiUpdatesInDB);

            var updateChecksInDB = from UpdateCheckRecord updateCheck in DatabaseHelper.FasDataContext.UpdateChecks select updateCheck;
            AllUpdateChecks = new ObservableCollection<UpdateCheckRecord>(updateChecksInDB);


            var playerInDB = from PlayerRecord player in DatabaseHelper.FasDataContext.Players select player;
            AllPlayers = new ObservableCollection<PlayerRecord>(playerInDB);

            var clubsInDB = from ClubRecord club in DatabaseHelper.FasDataContext.Clubs select club;
            AllClubs = new ObservableCollection<ClubRecord>(clubsInDB);

            var zonesInDB = from ZoneRecord zone in DatabaseHelper.FasDataContext.Zones select zone;
            AllZones = new ObservableCollection<ZoneRecord>(zonesInDB);


            var countriesInDB = from CountryRecord country in DatabaseHelper.FasDataContext.Countries select country;
            AllCountries = new ObservableCollection<CountryRecord>(countriesInDB);

            var continentsInDB = from ContinentRecord continent in DatabaseHelper.FasDataContext.Continents select continent;
            AllContinents = new ObservableCollection<ContinentRecord>(continentsInDB);

            var positionsInDB = from PositionRecord position in DatabaseHelper.FasDataContext.Positions select position;
            AllPositions = new ObservableCollection<PositionRecord>(positionsInDB);

            var packagesInDB = from PackageRecord package in DatabaseHelper.FasDataContext.Packages select package;
            AllPackages = new ObservableCollection<PackageRecord>(packagesInDB);

            var leaguesInDB = from LeagueRecord league in DatabaseHelper.FasDataContext.Leagues select league;
            AllLeagues = new ObservableCollection<LeagueRecord>(leaguesInDB);

        }

        #endregion


        #region Login / Logout

        /// <summary>
        /// Login called from the register routine
        /// </summary>
        internal void Login(JsonRegisterDetails registerDetails)
        {
            // Login
            LoginUser(registerDetails.User);

            // Put the new votes in the database
            ReplaceVotes(registerDetails.Votes);

            // Update last and totalvotespurchased
            UpdateVoteData(0, registerDetails.Votes.Count);

            // Set the live tile
            LiveTiles.Set(AvailableVotesCount(), TotalVotesPurchased() - AvailableVotesCount());
        }


        /// <summary>
        /// Login by creating a user and then adding the votes to the database
        /// </summary>
        internal void Login(JsonLoginDetails loginDetails)
        {
            // Login
            LoginUser(loginDetails.User);
            
            // Process the current pitch object
            UpdateCurrentPitch(loginDetails.Positions);

            // Put the new votes in the database
            ReplaceVotes(loginDetails.Votes);

            // Update last and totalvotespurchased
            UpdateVoteData(loginDetails.Last, loginDetails.TotalVotesPurchased);

            // Set the live tile
            LiveTiles.Set(AvailableVotesCount(), TotalVotesPurchased() - AvailableVotesCount());
        }

        /// <summary>
        /// Login a user
        /// </summary>
        private void LoginUser(JsonUser loginDetails)
        {
            // Logout just in case
            Logout();

            // Create a new user
            CurrentUser = new UserRecord()
            {
                UserId = loginDetails.id,
                Nid = loginDetails.Nid,
                Email = loginDetails.Email,
                Fav1 = Convert.ToInt32(loginDetails.Fav1),
                Fav2 = Convert.ToInt32(loginDetails.Fav2),
                MyCountry = Convert.ToInt32(loginDetails.Country),
                FirstName = loginDetails.Name,
                //SubmittedAnswers = loginDetails.SubmittedAnswers,
                EntryAvailable =  loginDetails.EntryAvailable,
                LastName = loginDetails.Surname,
                IsValidated = loginDetails.Validated,
                Mobile = loginDetails.Mobile,
                TeamName = loginDetails.Nickname,
                LastAvailableVoteHistoryRetrievalTime = 0,
                LastSquadHistoryRetrievalTime = 0,
                LastLocalCastTime = DateTime.Now,
            };

            // Add user to the data context.
            DatabaseHelper.FasDataContext.Users.InsertOnSubmit(CurrentUser);

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }

        /// <summary>
        /// Logout by deleting user, votes and transactions from the database
        /// </summary>
        internal void Logout()
        {
            // Delete users, votes, transactions
            DatabaseHelper.FasDataContext.Users.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Users);
            DatabaseHelper.FasDataContext.Votes.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Votes);
            DatabaseHelper.FasDataContext.Transactions.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Transactions);
            DatabaseHelper.FasDataContext.PitchLocation.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.PitchLocation);
            DatabaseHelper.FasDataContext.Pitches.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Pitches);
            DatabaseHelper.FasDataContext.CurrentPitchLocations.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.CurrentPitchLocations);
            DatabaseHelper.FasDataContext.SubmitChanges();

            // Remove the local in memory versions as well
            CurrentUser = null;
            AllCurrentPitchLocations = new ObservableCollection<CurrentPitchLocationRecord>();
            AllTransactions = new ObservableCollection<TransactionRecord>();
            AllVotes = new ObservableCollection<VoteRecord>();
            AllPitchLocations = new ObservableCollection<PitchLocationRecord>();
            AllPitches = new ObservableCollection<PitchRecord>();

            // Set the live tile
            LiveTiles.Reset();
        }

        internal bool IsLoggedOn()
        {
            return CurrentUser != null;
        }


        internal bool CompareValidated(ValidatedResponse validatedResponse)
        {
            return CurrentUser.IsValidated != validatedResponse.Validated.Validated;
        }

        internal void UpdateValidated(ValidatedResponse validatedResponse)
        {
            CurrentUser.IsValidated = validatedResponse.Validated.Validated;

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }

        #endregion


        #region Pitch Queries

        /// <summary>
        /// Given a position, return the unique player - created and stored in pitch dictionary
        /// </summary>
        public PlayerRecord LastUniquePlayerByPosition(PositionRecord position)
        {
            if (!IsLoggedOn())
            {
                return null;
            }

            CurrentPitchLocationRecord plr = (from pl in AllCurrentPitchLocations
                                       where pl.PositionId == position.PositionId
                                       select pl).FirstOrDefault();

            if (plr != null)
            {
                return Player(plr.PlayerId);
            }

            return null;
        }

        /// <summary>
        /// Clear the current pitch
        /// </summary>
        internal void ClearCurrentPitch()
        {
            DatabaseHelper.FasDataContext.CurrentPitchLocations.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.CurrentPitchLocations);
            DatabaseHelper.FasDataContext.SubmitChanges();

            AllCurrentPitchLocations = new ObservableCollection<CurrentPitchLocationRecord>();

            // Refresh the main page icons if we are on that page
            RefreshMainPage();
        }

        /// <summary>
        /// Given a new pitch ( from a login or a vote for example ), save it to the database
        /// </summary>
        public void UpdateCurrentPitch(List<JsonPitchPosition> pitchPositions)
        {
            // Delete all pitch positions associated with the current pitch
            ClearCurrentPitch();

            var positionEntities = new EntitySet<CurrentPitchLocationRecord>();

            foreach (JsonPitchPosition position in pitchPositions)
            {
                CurrentPitchLocationRecord vr = new CurrentPitchLocationRecord() { PlayerId = position.PlayerId, PositionId = position.PositionId };

                positionEntities.Add(vr);

                AllCurrentPitchLocations.Add(vr);
            }

            if ( positionEntities.Count > 0 )
            {
                // Add an item to the data context.
                DatabaseHelper.FasDataContext.CurrentPitchLocations.InsertAllOnSubmit(positionEntities);

                // Save changes to the database.
                DatabaseHelper.FasDataContext.SubmitChanges();
            }

            // Refresh the main page icons if we are on that page
            RefreshMainPage();
        }


        /// <summary>
        /// Given a list of pictches from the server, update our database
        /// </summary>
        internal void UpdateHistoricalPitches(PitchHistoryResponse pitchDetails)
        {
            if (pitchDetails.PitchHistory.Pitches.Count <= 0)
                return;

            DatabaseHelper.FasDataContext.PitchLocation.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.PitchLocation);
            DatabaseHelper.FasDataContext.Pitches.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Pitches);
            DatabaseHelper.FasDataContext.SubmitChanges();

            AllPitches = new ObservableCollection<PitchRecord>();
            AllPitchLocations = new ObservableCollection<PitchLocationRecord>();

            var pitchEntities = new EntitySet<PitchRecord>();
            var pitchLocationEntities = new EntitySet<PitchLocationRecord>();

            foreach (PitchHistoryResponse.Pitch pitch in pitchDetails.PitchHistory.Pitches)
            {
                PitchRecord pitchRecord = new PitchRecord() { TimeStamp = pitch.Timestamp };

                pitchEntities.Add(pitchRecord);

                // Add the item to the observable collection.
                AllPitches.Add(pitchRecord);

                // Add the positions
                foreach (JsonPitchPosition pos in pitch.Positions)
                {
                    PitchLocationRecord pitchLocation = new PitchLocationRecord() { Pitch = pitchRecord, PlayerId = Convert.ToInt32(pos.player_id), PositionId = Convert.ToInt32(pos.position_id) };

                    pitchRecord.Pitch.Add(pitchLocation);

                    pitchLocationEntities.Add(pitchLocation);

                    AllPitchLocations.Add(pitchLocation);
                }

            }

            var historicalPitches = from pitch in AllPitches
                                    orderby pitch.TimeStamp ascending
                                    select pitch;

            // The fist vote is the global vote
            var first = historicalPitches.FirstOrDefault();
            if (first != null)
            {
                first.Status = "Global Vote";
            }

            int i = 1;
            foreach (var x in historicalPitches)
            {
                x.Name = "Team " + i;
                i += 1;
            }

            // Add an item to the data context.
            DatabaseHelper.FasDataContext.PitchLocation.InsertAllOnSubmit(pitchLocationEntities);
            DatabaseHelper.FasDataContext.Pitches.InsertAllOnSubmit(pitchEntities);

            // Record what time we saved all of this data
            App.ViewModel.DbViewModel.CurrentUser.LastSquadHistoryRetrievalTime = pitchDetails.PitchHistory.LastUpdate;

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }

        internal PitchRecord GetPitch(int id)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            return (from pitch in AllPitches
                    where pitch.PitchId == id
                    select pitch).FirstOrDefault();
        }

        /// <summary>
        /// Given transactions from the database, has any changed. This is used in the polling of the transaction every 30 seconds to see if the user has made a purchase
        /// </summary>
        internal bool PitchesRequireSyncing(PitchHistoryResponse pitchDetails)
        {
            // For the moment, just return true if the number of pitches has increased indicating they have cast a team
            return pitchDetails.PitchHistory.Pitches.Count > DatabaseHelper.FasDataContext.Pitches.Count();
        }



        /// <summary>
        /// Get all the pitches for teh history page
        /// </summary>
        public IEnumerable PitchHistory()
        {
            int count = AllPitches.Count;

            if (App.AppConstants.ApplyingUpdates) return null;

            return from pitch in DatabaseHelper.FasDataContext.Pitches
                                       orderby pitch.TimeStamp descending
                                        select pitch;
        }

        #endregion


        #region Transaction Queries

        /// <summary>
        /// Given transactions from the database, has any changed. This is used in the polling of the transaction every 30 seconds to see if the user has made a purchase
        /// </summary>
        internal bool TransactionsRequireSyncing(TransactionsResponse transDetails)
        {
            // For the moment, just return true if the number of transactions has increased indicating they have made a purchase
            return transDetails.Transactions.Count > DatabaseHelper.FasDataContext.Transactions.Count();
        }

        /// <summary>
        /// Given a list of transactions from the server, update our database
        /// </summary>
        internal void UpdateTransactions(TransactionsResponse transDetails)
        {
            if (transDetails.Transactions.Count <= 0)
                return;

            DatabaseHelper.FasDataContext.Transactions.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Transactions);
            DatabaseHelper.FasDataContext.SubmitChanges();

            AllTransactions = new ObservableCollection<TransactionRecord>();

            var transEntities = new EntitySet<TransactionRecord>();

            foreach (JsonTransaction trans in transDetails.Transactions)
            {
                TransactionRecord tr = new TransactionRecord() { TransactionId = trans.id, PackageId = trans.PackageId, Origin = trans.Origin, Amount = trans.Amount, TimeStamp = trans.Timestamp };

                transEntities.Add(tr);

                // Add the item to the observable collection.
                AllTransactions.Add(tr);
            }

            // Add an item to the data context.
            DatabaseHelper.FasDataContext.Transactions.InsertAllOnSubmit(transEntities);

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }

        /// <summary>
        /// For the transaction history page
        /// </summary>
        public System.Collections.IEnumerable TransactionHistory()
        {
            System.Collections.Generic.IEnumerable<TransactionHistoryRecord> temp = from transaction in AllTransactions
                                                                                    join package in AllPackages on transaction.PackageId equals package.PackageId

                                                                                           into newPachages
                                                                                    from newPackage in newPachages.DefaultIfEmpty()

                                                                                    where newPackage != null

                                                                                    select new TransactionHistoryRecord() { PackageRecord = newPackage, TransactionRecord = transaction, TransactionTime = DateTimeHelper.DateTimeFromUnixTimestampMillis(transaction.TimeStamp) };


            return from trans in temp
                   group trans by trans.TransactionTime into groupedTransactions
                   orderby groupedTransactions.Key descending
                   select new PublicGrouping<DateTime, TransactionHistoryRecord>(groupedTransactions);
        }

        #endregion


        #region Vote Queries

        /// <summary>
        /// Given a list of votes from the server, update our database
        /// Normally called from loing and register where we completely replace votes
        /// </summary>
        internal void ReplaceVotes(List<JsonVote> votes)
        {
            if (votes.Count <= 0)
                return;

            DatabaseHelper.FasDataContext.Votes.DeleteAllOnSubmit(DatabaseHelper.FasDataContext.Votes);
            DatabaseHelper.FasDataContext.SubmitChanges();

            AllVotes = new ObservableCollection<VoteRecord>();

            var voteEntities = new EntitySet<VoteRecord>();

            foreach (JsonVote vote in votes)
            {
                VoteRecord vr = new VoteRecord() { VoteId = vote.id, PlayerId = vote.PlayerId, PositionId = vote.PositionId, TimeStamp = vote.Timestamp, Created = vote.Created };

                voteEntities.Add(vr);

                // Add a vote item to the observable collection.
                AllVotes.Add(vr);
            }

            // Add an item to the data context.
            DatabaseHelper.FasDataContext.Votes.InsertAllOnSubmit(voteEntities);

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }

        /// <summary>
        /// Merge available votes
        /// Usually called when a response from /vote/1 is returned - which just returns AVAILABLE votes
        /// </summary>
        public bool MergeVotes(VotesAvailableResponse votesResponse)
        {
#if DEBUG
            DateTime start = DateTime.Now;
#endif
            // get votes from the available response
            List<JsonVote> availableVotes = votesResponse.Details.Votes;
            
            // Do the available votes merge
            var voteEntities = MergeVotes(availableVotes);

            // Update last and totalvotespurchased
            UpdateVoteData(votesResponse.Details.Last, votesResponse.Details.TotalVotesPurchased);

#if DEBUG
            TimeSpan length = DateTime.Now - start;
            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "MergeVotes (ms) " + length.TotalMilliseconds }, new TimeSpan(0, 0, 3)));
#endif
            return (voteEntities.Count > 0);
        }

        /// <summary>
        /// Merge a list of AVAILABLE votes
        /// </summary>
        public EntitySet<VoteRecord> MergeVotes(List<JsonVote> availableVotes)
        {
            // to hold any new votes
            var voteEntities = new EntitySet<VoteRecord>();

            if (availableVotes.Count > 0)
            {
                // Add votes to a dictionary for rapid finding
                var voteDictionary = new Dictionary<string, VoteRecord>();
                foreach (VoteRecord vote in AllVotes)
                {
                    voteDictionary.Add(vote.VoteId, vote);
                }

                // Now iterate all of the response json votes and for all that are populated, make sure our version is populated too
                foreach (JsonVote serverVote in availableVotes)
                {
                    // Find the vote locally
                    if (!voteDictionary.ContainsKey(serverVote.id))
                    {
                        // if it doesn't exist then create it - the user must have bought some votes
                        VoteRecord newLocalVote = new VoteRecord() { VoteId = serverVote.id, PlayerId = serverVote.PlayerId, PositionId = serverVote.PositionId, Created = serverVote.Created, TimeStamp = serverVote.Timestamp };
                        voteEntities.Add(newLocalVote);
                        AllVotes.Add(newLocalVote);
                    }
                    else
                    {
                        // If it already exists then no need to create it - as we are only merging in AVAILABLE votes
                    }
                }

                // If there are any new votes then add them
                if (voteEntities.Count > 0)
                {
                    DatabaseHelper.FasDataContext.Votes.InsertAllOnSubmit(voteEntities);
                }

                if (voteEntities.Count > 0)
                {
                    // Save changes to the database.
                    DatabaseHelper.FasDataContext.SubmitChanges();

                    // Update the live tile
                    LiveTiles.Set(AvailableVotesCount(), TotalVotesPurchased() - AvailableVotesCount());

                    // If we are on the main page then update the icons
                    // No need to as this is only AVAILABLE votes
                }
            }
            return voteEntities;
        }


        /// <summary>
        /// Given a votes response, merge them with our local database votes
        /// This merges ALL votes - from /vote
        /// </summary>
        public bool MergeVotes(VotesAllResponse votesResponse)
        {
            DateTime start = DateTime.Now;
            bool requiresSyncing = false;

            // Add votes to a dictionary for rapid finding
            var voteDictionary = new Dictionary<string, VoteRecord>();
            foreach (VoteRecord vote in AllVotes)
            {
                voteDictionary.Add(vote.VoteId, vote);
            }

            // to hold any new votes
            var voteEntities = new EntitySet<VoteRecord>();

            // Now iterate all of the response json votes and for all that are populated, make sure our version is populated too
            foreach (JsonVote serverVote in votesResponse.Votes)
            {
                //VoteRecord localVote = AllVotes.FirstOrDefault(v => v.VoteId == serverVote.id); - too slow - in a dictionary instead

                // Find the vote locally
                if (!voteDictionary.ContainsKey(serverVote.id))
                {
                    // if it doesn't exist then create it - the user must have bought some votes
                    VoteRecord newLocalVote = new VoteRecord() { VoteId = serverVote.id, PlayerId = serverVote.PlayerId, PositionId = serverVote.PositionId, TimeStamp = serverVote.Timestamp, Created = serverVote.Created };
                    voteEntities.Add(newLocalVote);
                    AllVotes.Add(newLocalVote);
                }
                else
                {
                    // Get the existing vote
                    VoteRecord localVote = voteDictionary[serverVote.id];

                    // check that the server vote isnt empty - otherwise we may overwrite a locally used vote with an empty vote
                    if (serverVote.PlayerId != 0 && serverVote.PositionId != 0 && serverVote.Timestamp != 0)
                    {
                        // other than that, if the vote is different then update it
                        if (localVote.PlayerId != serverVote.PlayerId && localVote.PositionId != serverVote.PositionId && localVote.TimeStamp != serverVote.Timestamp)
                        {
                            requiresSyncing = true;

                            // Sync it - the user must have used a vote
                            localVote.PlayerId = serverVote.PlayerId;
                            localVote.PositionId = serverVote.PositionId;
                            localVote.TimeStamp = serverVote.Timestamp;
                        }
                    }
                }
            }

            // If we were using the latest api, we could use this...
            // Record what time we saved all of this data
            //App.ViewModel.DbViewModel.CurrentUser.LastVoteHistoryRetrievalTime = votesResponse.Votes......LastUpdate;


            // If there are any new votes then add them
            if (voteEntities.Count > 0)
            {
                DatabaseHelper.FasDataContext.Votes.InsertAllOnSubmit(voteEntities);
            }

            if (requiresSyncing || voteEntities.Count > 0)
            {
                // Save changes to the database.
                DatabaseHelper.FasDataContext.SubmitChanges();

                // Update the live tile
                LiveTiles.Set(AvailableVotesCount(), TotalVotesPurchased() - AvailableVotesCount());

                // If we are on the main page then update the icons
                RefreshMainPage();
            }

#if DEBUG
            TimeSpan length = DateTime.Now - start;
            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "MergeVotes (ms) " + length.TotalMilliseconds }, new TimeSpan(0, 0, 3)));
#endif
            return (requiresSyncing || voteEntities.Count > 0);
        }

        /// <summary>
        /// This is now stored in the user object
        /// </summary>
        internal int TotalVotesPurchased()
        {
            if ( IsLoggedOn() )
            {
                return CurrentUser.TotalVotesPurchased;
            }

            return 0;
        }

        /// <summary>
        /// Total number of available votes taking into account the pending time
        /// </summary>
        internal int AvailableVotesCount()
        {
            long nowMinusPendingTime = DateTimeHelper.GetUnixTimestampMillis(DateTime.Now - TimeSpan.FromSeconds(App.AppConstants.SecondsToAllowAPendingVote));

            return (from vote in AllVotes
                    where (vote.LocalCastTimeStamp == 0 || vote.LocalCastTimeStamp < nowMinusPendingTime)
                    where vote.PlayerId == 0
                    where vote.PositionId == 0
                    select vote).Count();
        }


        /// <summary>
        /// get the next available vote to cast
        /// </summary>
        public VoteRecord FindNextAvailableVote()
        {
            long nowMinusPendingTime = DateTimeHelper.GetUnixTimestampMillis(DateTime.Now - TimeSpan.FromSeconds(App.AppConstants.SecondsToAllowAPendingVote));

            // we want empty votes or one where the timestamp is more than 30 seconds ago
            return (from vote in AllVotes
                    where (vote.LocalCastTimeStamp == 0 || vote.LocalCastTimeStamp < nowMinusPendingTime)
                    where vote.PlayerId == 0
                    where vote.PositionId == 0
                    select vote).FirstOrDefault();
        }


        /// <summary>
        /// When we cast a vote, we mark it as pending so another attempted cast cannot grab it
        /// </summary>
        /// <param name="vote"></param>
        internal void MarkVoteAsPending(VoteRecord vote)
        {
            // Mark the timestamp as now
            vote.LocalCastTimeStamp = DateTimeHelper.GetUnixTimestampMillis(DateTime.Now);

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }


        /// <summary>
        /// The server has successfully responded with success - so update that vote
        /// </summary>
        internal void VoteSuccessfullyCast(JsonVoteDetails voteDetails)
        {
            if (!IsLoggedOn())
            {
                return;
            }

            JsonVote vote = voteDetails.Vote;

            VoteRecord voteCast = FindVoteWithId(vote.id);

            // Update the vote
            if (voteCast != null)
            {
                // Record the last local vote time
                CurrentUser.LastLocalCastTime = DateTime.Now;

                voteCast.PlayerId = vote.PlayerId;
                voteCast.PositionId = vote.PositionId;
                voteCast.TimeStamp = vote.Timestamp;

                // Save changes to the database.
                DatabaseHelper.FasDataContext.SubmitChanges();
            }

            // Process the current pitch object
            UpdateCurrentPitch(voteDetails.Positions);

            // Refresh the main page icons if we are on that page
            RefreshMainPage();

            // Update the live tile
            LiveTiles.Set(AvailableVotesCount(), TotalVotesPurchased() - AvailableVotesCount());
        }

        /// <summary>
        /// Update the vote meta data
        /// </summary>
        public void UpdateVoteData(long lastRetrievalTime, int totalVotesPurchased)
        {
            bool changed = false;

            if (App.ViewModel.DbViewModel.CurrentUser.LastAvailableVoteHistoryRetrievalTime != lastRetrievalTime)
            {
                // Save the last time we accessed this api
                App.ViewModel.DbViewModel.CurrentUser.LastAvailableVoteHistoryRetrievalTime = lastRetrievalTime;
                changed = true;
            }

            if (App.ViewModel.DbViewModel.CurrentUser.TotalVotesPurchased != totalVotesPurchased)
            {
                // Save the total number of votes this user has purchased to feed back into this api
                App.ViewModel.DbViewModel.CurrentUser.TotalVotesPurchased = totalVotesPurchased;
                changed = true;
            }

            if (changed)
            {
                // Save changes to the database.
                DatabaseHelper.FasDataContext.SubmitChanges();
            }
        }

        /// <summary>
        /// For the voting history page
        /// </summary>
        public System.Collections.IEnumerable VotingHistory()
        {
            System.Collections.Generic.IEnumerable<VotingHistoryRecord> temp = from vote in AllVotes
                                                                               join player in AllPlayers on vote.PlayerId equals player.PlayerId

                                                                                   into newPlayers
                                                                               from newPlayer in newPlayers.DefaultIfEmpty()

                                                                               join position in AllPositions on vote.PositionId equals position.PositionId
                                                                                                    into newPositions

                                                                               from newPosition in newPositions.DefaultIfEmpty()

                                                                               where newPlayer != null

                                                                               orderby newPlayer.LastName ascending
                                                                               //orderby vote.TimeStamp descending

                                                                               select new VotingHistoryRecord() 
                                                                               { 
                                                                                   PlayerRecord = newPlayer, 
                                                                                   Position = newPosition == null ? "unknown" : newPosition.Key, 
                                                                                   VoteTime = DateTimeHelper.DateTimeFromUnixTimestampMillis(vote.TimeStamp) 
                                                                               };



            return from player in temp
                   group player by PlayerNameChooseSortLetter(player.PlayerRecord) into groupedPlayers
                   //group player by player.Position into groupedPlayers
                   orderby groupedPlayers.Key
                   select new PublicGrouping<string, VotingHistoryRecord>(groupedPlayers);
        }

        /// <summary>
        /// Given a vote id, return the vote record
        /// </summary>
        private VoteRecord FindVoteWithId(string id)
        {
            return (from vote in AllVotes
                    where vote.VoteId == id
                    select vote).FirstOrDefault();
        }

        #endregion


        #region Vote Simulation and Old Vote Stuff

        internal void CastSimulatedVote(int playerid, int positionid)
        {
            VoteRecord voteCast = new VoteRecord() { VoteId = Guid.NewGuid().ToString(), PlayerId = playerid, PositionId = positionid, TimeStamp = DateTimeHelper.GetUnixTimestampMillis(DateTime.Now) };

            AllVotes.Add(voteCast);

            DatabaseHelper.FasDataContext.Votes.InsertOnSubmit(voteCast);

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();

            // Refresh the main page icons if we are on that page
            RefreshMainPage();
        }


        /// <summary>
        /// Given votes from the database, has any changed. This is used in the polling of the votes every 30 seconds to see if the user has made a purchase
        /// </summary>
        internal bool VotesRequireSyncing(List<JsonVote> serverVotes)
        {
            DateTime start = DateTime.Now;

            // Find out if this vote response actually belongs to the logged in user - this is to make sure that the user issuing the original response is still the logged in user
            bool myVotes = false;
            VoteRecord firstVote = AllVotes.FirstOrDefault();
            if (firstVote != null)
            {
                JsonVote jsonVote = serverVotes.FirstOrDefault(v => v.id == firstVote.VoteId);
                if (jsonVote != null)
                {
                    myVotes = true;
                }
            }
#if DEBUG
            //            TimeSpan length = DateTime.Now - start;
            //            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "VotesRequireSyncing (ms) " + length.TotalMilliseconds }, new TimeSpan(0, 0, 3)));
#endif

            // if one of our votes isn't in the response then just return
            if (!myVotes)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Given all of the cast votes, work out who should be drawn on the pitch
        /// </summary>
        //public void ReCalculatePitchPositions()
        //{
        /*
        positionsDictionary.Clear();
        playersDictionary.Clear();
        pitchDictionary.Clear();

        var votescast = from vote in AllVotes
                        where vote.PlayerId != 0
                        where vote.PositionId != 0
                        where vote.TimeStamp != 0
                        orderby vote.TimeStamp descending
                        select vote;

        foreach (VoteRecord vote in votescast)
        {
            if (!positionsDictionary.ContainsKey(vote.PositionId) && !playersDictionary.ContainsKey(vote.PlayerId))
            {
                pitchDictionary.Add(vote.PositionId, vote.PlayerId);
                positionsDictionary.Add(vote.PositionId, 0);
                playersDictionary.Add(vote.PlayerId, 0);
            }
        }
        */

        // Here's Jasons algorithm that maps votes to pitch positions in some kind of sudo code:

        //votes = getAllCastVotesOrderedByCastTimeDescending()

        //players = positions = pitch = array()

        //foreach (votes as vote)
        //{
        //    if(!in_array(vote.position, positions) && !in_array(vote.player, players))
        //    {
        //            pitch = array(vote.position, vote.player)
        //            players.push(vote.player)
        //            positions.push(vote.position)
        //    }
        //}

        // Basically, get the cast votes ordered descending by time, if the position or player has been seen before skip adding it to your position array.
        //}



        /// <summary>
        /// Used for the player icon screen to find all the cast votes so they can draw them
        /// </summary>
        //public IEnumerable<VoteRecord> VotesCast(ListSortDirection sortDirection)
        //{
        //    switch (sortDirection)
        //    {
        //        case ListSortDirection.Ascending:

        //            return
        //                    from vote in AllVotes
        //                    where vote.PlayerId != 0
        //                    where vote.PositionId != 0
        //                    where vote.TimeStamp != 0
        //                    orderby vote.TimeStamp ascending
        //                    select vote;
        //            break;

        //        case ListSortDirection.Descending:

        //            return
        //                    from vote in AllVotes
        //                    where vote.PlayerId != 0
        //                    where vote.PositionId != 0
        //                    where vote.TimeStamp != 0
        //                    orderby vote.TimeStamp descending
        //                    select vote;

        //            break;

        //    }

        //    return null;
        //}

/*
        /// <summary>
        /// Given a position, just return the last vote for this position
        /// </summary>
        public PlayerRecord LastVoteCastByPosition(PositionRecord position)
        {
            var votes =
                    from vote in AllVotes
                    where vote.PlayerId != 0
                    where vote.PositionId == position.PositionId
                    where vote.TimeStamp != 0
                    orderby vote.TimeStamp descending
                    select vote;

            VoteRecord lastVote = votes.FirstOrDefault();

            if (lastVote != null)
            {
                PlayerRecord player = App.ViewModel.DbViewModel.Player(lastVote.PlayerId);
                return player;
            }
            return null;
        }

        public int CountVotesCastByPosition(PositionRecord position)
        {
            var votes =
                    from vote in AllVotes
                    where vote.PlayerId != 0
                    where vote.PositionId == position.PositionId
                    where vote.TimeStamp != 0
                    select vote;

            return votes.Count();
        }        
*/
        #endregion


        #region Verify

        internal void Validate(string uuid)
        {
            // Check this in case they have logged out whilst waiting for the response
            if (CurrentUser.UserId == uuid)
            {
                // update validated
                CurrentUser.IsValidated = true;

                // Save changes to the database.
                DatabaseHelper.FasDataContext.SubmitChanges();
            }
        }

        #endregion


        #region Update User

        internal void UpdateUser(JsonUser jsonUser)
        {
            // update the current user in the database
            CurrentUser.FirstName = jsonUser.Name;
            CurrentUser.LastName = jsonUser.Surname;
            CurrentUser.Email = jsonUser.Email;
            CurrentUser.Fav1 = Convert.ToInt32(jsonUser.Fav1);
            CurrentUser.Fav2 = Convert.ToInt32(jsonUser.Fav2);
            CurrentUser.MyCountry = Convert.ToInt32(jsonUser.Country);
            CurrentUser.IsValidated = jsonUser.Validated;
            CurrentUser.Mobile = jsonUser.Mobile;
            CurrentUser.TeamName = jsonUser.Nickname;
            CurrentUser.Nid = jsonUser.Nid;
            //CurrentUser.SubmittedAnswers = jsonUser.SubmittedAnswers;
            CurrentUser.EntryAvailable = jsonUser.EntryAvailable;

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }


        internal void UpdateUserEntryAvailable(bool available)
        {
            if (IsLoggedOn())
            {
                if (CurrentUser.EntryAvailable != available)
                {
                    CurrentUser.EntryAvailable = available;

                    // Save changes to the database.
                    DatabaseHelper.FasDataContext.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Return the loged in users favourite club
        /// </summary>
        internal ClubRecord FirstFavouriteClub()
        {
            if (IsLoggedOn())
            {
                return Club(CurrentUser.Fav1);
            }

            return null;
        }

        /// <summary>
        /// Return the loged in users favourite club
        /// </summary>
        internal ClubRecord SecondFavouriteClub()
        {
            if (IsLoggedOn())
            {
                return Club(CurrentUser.Fav2);
            }

            return null;
        }

        /// <summary>
        /// Get the logged in users country
        /// </summary>
        internal CountryRecord MyCountry()
        {
            if (IsLoggedOn())
            {
                return Country(CurrentUser.MyCountry);
            }

            return null;
        }

        #endregion


        #region Club Queries

        public IEnumerable<PublicGrouping<string, ClubRecord>> ClubsListForSearch()
        {
            IEnumerable<ClubRecord> sortedClubs = null;

            sortedClubs =
                from club in AllClubs
                where club.ClubId != DatabaseHelper.DummyId
                where club.PlayerCount > 0
                orderby club.Name ascending
                select club;

            var x = from club in sortedClubs
                    group club by ClubNameChooseSortLetter(club) into clubGroups
                    select new PublicGrouping<string, ClubRecord>(clubGroups);

            return x;
        }

        /// <summary>
        /// For the favourite club page
        /// </summary>
        public IEnumerable<PublicGrouping<string, ClubRecord>> ClubsListForFavouriteClub(ListSortDirection sortDirection, string searchTerm)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            IEnumerable<ClubRecord> sortedClubs = null;

            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    {

                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            sortedClubs =
                                    from club in AllClubs
                                    where club.ClubId != DatabaseHelper.DummyId
                                    where !string.IsNullOrEmpty(club.Country)
                                    orderby club.Name ascending
                                    select club;
                        }
                        else
                        {
                            sortedClubs =
                                    from club in AllClubs
                                    where club.ClubId != DatabaseHelper.DummyId
                                    where club.Name.ToLower().Contains(searchTerm.ToLower())
                                    where !string.IsNullOrEmpty(club.Country)
                                    orderby club.Name ascending
                                    select club;
                        }
                    }
                    break;

                case ListSortDirection.Descending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            sortedClubs =
                                    from club in AllClubs
                                    where club.ClubId != DatabaseHelper.DummyId
                                    where !string.IsNullOrEmpty(club.Country)
                                    orderby club.Name descending
                                    select club;
                        }
                        else
                        {
                            sortedClubs =
                                    from club in AllClubs
                                    where club.ClubId != DatabaseHelper.DummyId
                                    where club.Name.ToLower().Contains(searchTerm.ToLower())
                                    where !string.IsNullOrEmpty(club.Country)
                                    orderby club.Name descending
                                    select club;
                        }
                    }
                    break;
            }

            var x = from club in sortedClubs
                    group club by ClubNameChooseSortLetter(club) into clubGroups
                           select new PublicGrouping<string, ClubRecord>(clubGroups);

            return x;
        }

        /// <summary>
        /// For the favourite club page
        /// </summary>
        public int ClubsCount(string search)
        {
            return (from club in AllClubs
                    where club.ClubId != DatabaseHelper.DummyId
                    where club.Name.ToLower().Contains(search.ToLower())
                    where !string.IsNullOrEmpty(club.Country)
                    select club).Count();
        }

        /// <summary>
        /// Given a club id, return the club
        /// </summary>
        public ClubRecord Club(int id)
        {
                return (from club in AllClubs
                        where club.ClubId == id
                        select club).FirstOrDefault();
        }

        public void PrintClubsWithNoPlayers()
        {
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("Clubs with no players");
            System.Diagnostics.Debug.WriteLine("=====================");
            System.Diagnostics.Debug.WriteLine("");

            var emptyClubs =
                    from club in AllClubs
                    where club.ClubId != DatabaseHelper.DummyId
                    where club.Players.Count <= 0
                    orderby club.Name ascending
                    select club;

            System.Diagnostics.Debug.WriteLine(" Total: " + emptyClubs.Count());
            System.Diagnostics.Debug.WriteLine("");

            List<ClubRecord> list = emptyClubs.ToList();
            foreach (ClubRecord record in list)
            {
                System.Diagnostics.Debug.WriteLine(record.ClubId + " : " + record.Name);
            }
        }

        #endregion


        #region Countries Queries

        public System.Collections.IEnumerable CountriesListForSearch()
        {
            return from country in AllCountries
                   where country.CountryId != DatabaseHelper.DummyId
                   where country.SelectableInUserAccount(true)
                   where country.PlayerCount > 0
                   orderby country.Name ascending
                   group country by CountryNameChooseSortLetter(country) into c
                   select new PublicGrouping<string, CountryRecord>(c);
        }


        /// <summary>
        /// For the countries list page
        /// </summary>
        public System.Collections.IEnumerable CountriesKeyList(bool onlyWithPlayers, ListSortDirection sortDirection, string searchTerm)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            if (false)
            {
                System.Diagnostics.Debug.WriteLine("");
                System.Diagnostics.Debug.WriteLine("Countries with no players");
                System.Diagnostics.Debug.WriteLine("=========================");
                System.Diagnostics.Debug.WriteLine("");

                var noPlayers = from country in AllCountries
                                where country.CountryId != DatabaseHelper.DummyId
                                where country.SelectableInUserAccount(onlyWithPlayers)
                                where country.Players.Count <= 0
                                orderby country.Name ascending
                                select country;

                List<CountryRecord> list = noPlayers.ToList();
                foreach (CountryRecord record in list)
                {
                    System.Diagnostics.Debug.WriteLine(record.CountryId + " : " + record.Name);
                }
            }

            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from country in AllCountries
                                   where country.CountryId != DatabaseHelper.DummyId
                                   where country.SelectableInUserAccount(onlyWithPlayers)
                                   where country.Show(onlyWithPlayers)
                                   orderby country.Name ascending
                                   group country by CountryNameChooseSortLetter(country) into c
                                   select new PublicGrouping<string, CountryRecord>(c);
                        }
                        else
                        {
                            return from country in AllCountries
                                   where country.CountryId != DatabaseHelper.DummyId
                                   where country.SelectableInUserAccount(onlyWithPlayers)
                                   where country.Name.ToLower().Contains(searchTerm.ToLower())
                                   where country.Show(onlyWithPlayers)
                                   orderby country.Name ascending
                                   group country by CountryNameChooseSortLetter(country) into c
                                   select new PublicGrouping<string, CountryRecord>(c);
                        }
                    }
                    break;

                case ListSortDirection.Descending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from country in AllCountries
                                   where country.CountryId != DatabaseHelper.DummyId
                                   where country.SelectableInUserAccount(onlyWithPlayers)
                                   where country.Show(onlyWithPlayers)
                                   orderby country.Name descending
                                   group country by CountryNameChooseSortLetter(country) into c
                                   select new PublicGrouping<string, CountryRecord>(c);
                        }
                        else
                        {
                            return from country in AllCountries
                                   where country.CountryId != DatabaseHelper.DummyId
                                   where country.SelectableInUserAccount(onlyWithPlayers)
                                   where country.Name.ToLower().Contains(searchTerm.ToLower())
                                   where country.Show(onlyWithPlayers)
                                   orderby country.Name descending
                                   group country by CountryNameChooseSortLetter(country) into c
                                   select new PublicGrouping<string, CountryRecord>(c);
                        }
                    }
                    break;
            }

            return null;
            //return from country in AllCountries
            //       where country.CountryId != DatabaseHelper.DummyId
            //       orderby country.Name ascending
            //       group country by country.Name.Remove(1).ToUpper() into c
            //       orderby c.Key ascending
            //       select new PublicGrouping<string, CountryRecord>(c);
        }

        /// <summary>
        /// </summary>
        public CountryRecord Country(int id)
        {
            return (from country in AllCountries
                    where country.CountryId == id
                    select country).FirstOrDefault();
        }

        /// <summary>
        /// For the favourite club page
        /// </summary>
        public int CountriesCount(bool forUserAccount, string search)
        {
            return (from country in AllCountries
                    where country.CountryId != DatabaseHelper.DummyId
                    where country.Name.ToLower().Contains(search.ToLower())
                    where country.SelectableInUserAccount(forUserAccount)
                    select country).Count();
        }


        #endregion


        #region Zone Queries

        /// <summary>
        /// Given a zone id, return the zone
        /// </summary>
        public ZoneRecord Zone(int zoneId)
        {
            return (from zone in AllZones
                    where zone.ZoneId == zoneId
                    select zone).First();
        }

        /// <summary>
        /// Given a position, we want to know what zone it is in
        /// </summary>
        public ZoneRecord ZoneFromPosition(int positionId)
        {
            return (from position in AllPositions
                    where position.PositionId == positionId
                    select position.Zone).First();
        }

        /// <summary>
        /// Given a position key, return the zone
        /// </summary>
        internal ZoneRecord ZoneFromPosition(string positionKey)
        {
            return (from position in AllPositions
                    where position.Key == positionKey
                    select position.Zone).First();
        }

        /// <summary>
        /// Used for the zones list on the find page
        /// </summary>
        internal System.Collections.IEnumerable ZonesKeyList(ListSortDirection sortDirection, string searchTerm)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            return from zone in AllZones
                   where zone.ZoneId != DatabaseHelper.DummyId
                   orderby zone.ZoneId ascending
                   group zone by zone.Group into c
                   select new PublicGrouping<string, ZoneRecord>(c);
        }

        #endregion


        #region Positon Queries

        /// <summary>
        /// The player icon control needs to find a position by id so it can display it under the icon
        /// </summary>
        public PositionRecord Position(int positionId)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            return (from position in AllPositions
                    where position.PositionId == positionId
                    select position).FirstOrDefault();
        }

        /// <summary>
        /// Given a position key, return a position
        /// </summary>
        public PositionRecord Position(string key, bool isEurope)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            return (from position in AllPositions
                    where position.Key == key
                    where position.IsEurope == isEurope
                    select position).FirstOrDefault();
        }

        #endregion


        #region Player Queries

        internal bool MergeMoving(MovingResponse movingResponse)
        {
            if (App.AppConstants.ApplyingUpdates) return false;

            DateTime start = DateTime.Now;
            bool requiresSyncing = false;

            // Add players to a dictionary for rapid finding
            var playerDictionary = new Dictionary<int, PlayerRecord>();
            foreach (PlayerRecord player in AllPlayers)
            {
                playerDictionary.Add(player.PlayerId, player);
            }

            //if (false)
            //{
            //    // test - because /moving api is currently dead
            //    Random random = new Random();

            //    foreach (KeyValuePair<int, PlayerRecord> kvp in playerDictionary)
            //    {
            //        kvp.Value.Moving = random.Next(-1, 1);
            //        requiresSyncing = true;
            //    }
            //}
            //else
            //{
                // Now iterate all of the response json moving values
                foreach (JsonMoving serverMoving in movingResponse.Moving)
                {
                    // Find the player locally
                    if (playerDictionary.ContainsKey(serverMoving.PlayerId))
                    {
                        // Get the existing player
                        PlayerRecord localPlayer = playerDictionary[serverMoving.PlayerId];

                        // other than that, if the moving is different then update it
                        if (localPlayer.Moving != serverMoving.Moving)
                        {
                            requiresSyncing = true;

                            // Sync it
                            localPlayer.Moving = serverMoving.Moving;
                        }
                    }
                }
            //}

            if (requiresSyncing)
            {
                // Save changes to the database.
                DatabaseHelper.FasDataContext.SubmitChanges();
            }

#if DEBUG
            //TimeSpan length = DateTime.Now - start;
            //App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "Player moving (ms) " + length.TotalMilliseconds }, new TimeSpan(0, 0, 3)));
#endif
            return (requiresSyncing);
        }

        /// <summary>
        /// The player icon control needs to find a player by id
        /// </summary>
        public PlayerRecord Player(int id)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            return (from player in AllPlayers
                    where player.PlayerId == id
                    select player).FirstOrDefault();
        }

        internal System.Collections.IEnumerable PlayersList(ObservableCollection<CurrentPitchLocationRecord> pitch)
        {
            var players = new ObservableCollection<Zengo.WP8.FAS.ViewModels.TeamSubmitViewModel.PlayerPosition>();

            foreach (CurrentPitchLocationRecord cplr in pitch)
            {
                TeamSubmitViewModel.PlayerPosition pp = new TeamSubmitViewModel.PlayerPosition() 
                { 
                    Player = App.ViewModel.DbViewModel.Player(cplr.PlayerId),
                    Position = App.ViewModel.DbViewModel.Position(cplr.PositionId)
                };

                players.Add(pp);
            }

            return players;
        }

        /// <summary>
        /// List of players for the find page
        /// </summary>
        public System.Collections.IEnumerable PlayersList(ListSortDirection sortDirection, string searchTerm)
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from player in AllPlayers
                                   where player.PlayerId != DatabaseHelper.DummyId
                                   where player.IsDeleted == false
                                   orderby PlayerOrder(player) ascending
                                   group player by PlayerNameChooseSortLetter(player) into c
                                   orderby c.Key ascending
                                   select new PublicGrouping<string, PlayerRecord>(c);
                        }
                        else
                        {
                            return from player in AllPlayers
                                   where player.PlayerId != DatabaseHelper.DummyId
                                   where player.IsDeleted == false
                                   where player.LastName.ToLower().Contains(searchTerm.ToLower()) || player.FirstName.ToLower().Contains(searchTerm.ToLower()) 
                                   orderby PlayerOrder(player) ascending
                                   group player by PlayerNameChooseSortLetter(player) into c
                                   orderby c.Key ascending
                                   select new PublicGrouping<string, PlayerRecord>(c);
                        }
                    }
                    break;

                case ListSortDirection.Descending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from player in AllPlayers
                                   where player.PlayerId != DatabaseHelper.DummyId
                                   where player.IsDeleted == false
                                   orderby PlayerOrder(player) descending
                                   group player by PlayerNameChooseSortLetter(player) into c
                                   orderby c.Key descending
                                   select new PublicGrouping<string, PlayerRecord>(c);
                        }
                        else
                        {
                            return from player in AllPlayers
                                   where player.PlayerId != DatabaseHelper.DummyId
                                   where player.IsDeleted == false
                                   where player.LastName.ToLower().Contains(searchTerm.ToLower()) || player.FirstName.ToLower().Contains(searchTerm.ToLower()) 
                                   orderby PlayerOrder(player) descending
                                   group player by PlayerNameChooseSortLetter(player) into c
                                   orderby c.Key descending
                                   select new PublicGrouping<string, PlayerRecord>(c);
                        }
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public int PlayersCount(string search)
        {
            return (from player in AllPlayers
                    where player.PlayerId != DatabaseHelper.DummyId
                    where player.IsDeleted == false
                    where player.LastName.ToLower().Contains(search.ToLower()) || player.FirstName.ToLower().Contains(search.ToLower()) 
                    select player).Count();
        }

        /// <summary>
        /// All players from a particular zone
        /// </summary>
        /// <returns></returns>
        public System.Collections.IEnumerable PlayersList(ZoneRecord zone, bool? europe)
        {
            if (europe == null)
            {
                return from player in AllPlayers
                       where player.PlayerId != DatabaseHelper.DummyId
                           && player.Zone.ZoneId == zone.ZoneId
                            && player.IsDeleted == false
                       orderby PlayerOrder(player) ascending
                       group player by PlayerNameChooseSortLetter(player) into c
                       orderby c.Key
                       select new PublicGrouping<string, PlayerRecord>(c);
            }
            else
            {
                return from player in AllPlayers
                       where player.PlayerId != DatabaseHelper.DummyId
                           && player.Zone.ZoneId == zone.ZoneId
                               && player.IsEurope == europe
                                   && player.IsDeleted == false
                       orderby PlayerOrder(player) ascending
                       group player by PlayerNameChooseSortLetter(player) into c
                       orderby c.Key
                       select new PublicGrouping<string, PlayerRecord>(c);
            }
        }

        /// <summary>
        /// All players from a particular club
        /// </summary>
        public System.Collections.IEnumerable PlayersList(ClubRecord club)
        {
            return from player in AllPlayers
                   where player.PlayerId != DatabaseHelper.DummyId
                       && player.Club.ClubId == club.ClubId
                            && player.IsDeleted == false
                   //&& player.IsEurope == europe
                   orderby PlayerOrder(player) ascending
                   group player by PlayerNameChooseSortLetter(player) into c
                   orderby c.Key
                   select new PublicGrouping<string, PlayerRecord>(c);
        }

        /// <summary>
        /// All players from a particular country
        /// </summary>
        public System.Collections.IEnumerable PlayersList(CountryRecord country)
        {
            return from player in AllPlayers
                   where player.PlayerId != DatabaseHelper.DummyId
                       && player.Country.CountryId == country.CountryId
                            && player.IsDeleted == false
                   //&& player.IsEurope == europe
                   orderby PlayerOrder(player) ascending
                   group player by PlayerNameChooseSortLetter(player) into c
                   orderby c.Key
                   select new PublicGrouping<string, PlayerRecord>(c);
        }
        /// <summary>
        /// Used for the ordering above - works out what to order player by
        /// </summary>
        private static string PlayerOrder(PlayerRecord player)
        {
            if (!string.IsNullOrEmpty(player.LastName))
            {
                return player.LastName;
            }

            if (!string.IsNullOrEmpty(player.FirstName))
            {
                return player.FirstName;
            }

            return " ";
        }

        /// <summary>
        /// Used for the ordering above - works out what letter to group player by
        /// </summary>
        private static string PlayerNameChooseSortLetter(PlayerRecord player)
        {
            if ( !string.IsNullOrEmpty(player.LastName) )
            {
                return player.LastName.Remove(1).ToUpper();
            }

            if (!string.IsNullOrEmpty(player.FirstName))
            {
                return player.FirstName.Remove(1).ToUpper();
            }

            return " ";
        }



        public string ChoosePlayerName(PlayerRecord player)
        {
            if (!string.IsNullOrEmpty(player.LastName))
            {
                return player.LastName;
            }

            if (!string.IsNullOrEmpty(player.FirstName))
            {
                return player.FirstName;
            }

            return AppResources.NoName;
        }


        private static string ClubNameChooseSortLetter(ClubRecord club)
        {
            if (!string.IsNullOrEmpty(club.Name))
            {
                return club.Name.Remove(1).ToUpper();
            }
            return " ";
        }

        private static string CountryNameChooseSortLetter(CountryRecord country)
        {
            if (!string.IsNullOrEmpty(country.Name))
            {
                return country.Name.Remove(1).ToUpper();
            }
            return " ";
        }

        #endregion


        #region Packages Queries

        /// <summary>
        /// This is bound to the list control on the buy page
        /// </summary>
        internal List<PackageRecord> PackagesList()
        {
            if (App.AppConstants.ApplyingUpdates) return null;

            var x = from package in AllPackages
                   where package.PackageId != DatabaseHelper.DummyId
                   orderby package.Price descending 
                   select package;

            return x.ToList();
        }

        #endregion


        #region Updates Queries

        /// <summary>
        /// This is bound to the update checks page
        /// </summary>
        internal System.Collections.IEnumerable UpdatesAll()
        {
            return from updateCheck in AllUpdateChecks
                   group updateCheck 
                   by updateCheck.UpdateCheckId into c
                   orderby c.Key descending
                   select new PublicGrouping<int, UpdateCheckRecord>(c);
        }

        internal System.Collections.IEnumerable UpdatesWhereRequired()
        {
            return from updateCheck in AllUpdateChecks
                   where updateCheck.ApiCountNeeded > 0
                   group updateCheck
                   by updateCheck.UpdateCheckId into c
                   orderby c.Key descending
                   select new PublicGrouping<int, UpdateCheckRecord>(c);
        }

        /// <summary>
        /// Get the last successful update check (needed > 0 && needed == received && appliedsuccessfully)
        /// </summary>
        public UpdateCheckRecord LastSuccessfullUpdate()
        {
            return (from check in AllUpdateChecks
                    where check.ApiCountNeeded == check.Updates.Count
                    where check.ApiCountNeeded > 0
                    where check.AppliedSuccessfully == true
                    orderby check.DateTime descending
                    select check).FirstOrDefault();
        }

        #endregion


        #region Competition Stuff

        /// <summary>
        /// Should we enable the free question button
        /// </summary>
        public bool CanEnableFreeQuestion()
        {
            if ( !IsLoggedOn())
            {
                return false;
            }

            return CurrentUser.ShowFreeQuestion;
        }

        /// <summary>
        /// Call this when the free question has been answered so we mark their account as such
        /// </summary>
        public void FreeQuestionAnswered()
        {
            if (!IsLoggedOn())
            {
                return;
            }

            CurrentUser.SubmittedAnswers = true;

            // Save changes to the database.
            DatabaseHelper.FasDataContext.SubmitChanges();
        }

        #endregion


        /// <summary>
        /// If on the main page, reload the icons
        /// </summary>
        public void RefreshMainPage()
        {
            try
            {
                var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
                if (currentPage is MainPage)
                {
                    (currentPage as MainPage).ReloadPitchIcons();
                }
            }
            catch (Exception)
            {
            }
        }

        public bool OnMainPage()
        {
            var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
            if (currentPage is MainPage)
            {
                return true;
            }

            return false;
        }

        public bool OnPlayerListPage()
        {
            var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
            if (currentPage is PlayerListPage)
            {
                return true;
            }

            return false;
        }

        public bool OnPlayerFindPage()
        {
            var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
            if (currentPage is PlayerFindPage)
            {
                return true;
            }

            return false;
        }

        public bool OnFreeEntryPage()
        {
            var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
            if (currentPage is FreeEntryPage)
            {
                return true;
            }

            return false;
        }
    }
}
