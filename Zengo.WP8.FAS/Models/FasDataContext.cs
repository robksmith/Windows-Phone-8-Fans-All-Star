
#region usings

using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

#endregion

namespace Zengo.WP8.FAS.Models
{
    public class FasDataContext : DataContext
    {
        // Pass the connection string to the base class.
        public FasDataContext(string connectionString)
            : base(connectionString)
        { }

        // Users table
        public Table<UserRecord> Users;

        // Anon Users table
        public Table<UserAnonRecord> AnonUsers;


        // Votes table
        public Table<VoteRecord> Votes;

        // Transactions table
        public Table<TransactionRecord> Transactions;


        // Pitch location
        public Table<CurrentPitchLocationRecord> CurrentPitchLocations;



        // Pitch location
        public Table<PitchLocationRecord> PitchLocation;

        // Pitches
        public Table<PitchRecord> Pitches;



        // Updates table
        public Table<ApiUpdateRecord> ApiUpdates;

        // Update Check table
        public Table<UpdateCheckRecord> UpdateChecks;


        // Players table
        public Table<PlayerRecord> Players;

        // Clubs table
        public Table<ClubRecord> Clubs;

        // Leagues table
        public Table<LeagueRecord> Leagues;

        // Countries table
        public Table<CountryRecord> Countries;

        // Continents table
        public Table<ContinentRecord> Continents;

        // zones table
        public Table<ZoneRecord> Zones;

        // positions table
        public Table<PositionRecord> Positions;

        // packages table
        public Table<PackageRecord> Packages;
    }
}
