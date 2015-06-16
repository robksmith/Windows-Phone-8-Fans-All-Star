
#region Usings

using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using Newtonsoft.Json;
using System;
using System.Data.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Resources;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class DatabaseHelper
    {
        #region Fields

        // Force a database seed to build the initial database
        private static bool forceDatabaseSeed = false;

        // If forceDatabaseSeed above is set to true, what date do we want marked on the seed database
        //private static DateTime databaseSeedTime = new DateTime(2013, 3, 18, 10, 30, 0);
        //private static DateTime databaseSeedTime = new DateTime(2013, 5, 17, 10, 30, 0);
        //private static DateTime databaseSeedTime = new DateTime(2013, 5, 24, 15, 45, 0);
        private static DateTime databaseSeedTime = new DateTime(2013, 6, 20, 16, 00, 0);

        // If forceDatabaseSeed above is set to true, what is the location of the seed database
        //private static string seedLocation = "Json/Seed/18Mar2013";
        //private static string seedLocation = "Json/Seed/17May2013";
        //private static string seedLocation = "Json/Seed/24May2013";
        private static string seedLocation = "Json/Seed/20Jun2013";

        // Do we want regular update checks
        private static bool fireUpdates = true;

        // seconds between checks
        private static int secondsBetweenDataUpdateChecks = 300;
        private static int secondsBetweenUserUpdateChecks = 60;
        
        // where in the project directory is the database packaged
        private static string projectDbLocation = "Database/fas.sdf";

        // where do we want it in isolated storage
        private static string isolatedStorageDbLocation = "fas.sdf";

        // the connection string
        private static string dbConnectionString = "Data Source=isostore:/fas.sdf";

        // our dummy id for dummy records
        private static int dummyId = 999999;

        // the data context used to update the database
        private static FasDataContext fasDataContext;

        #endregion


        #region Properties

        public static string DBConnectionString { get { return dbConnectionString; } private set { } }

        public static int DummyId { get { return dummyId; } private set { } }

        public static bool FireUpdates { get { return fireUpdates; } private set { } }

        public static int SecondsBetweenDataUpdateChecks { get { return secondsBetweenDataUpdateChecks; } private set { } }

        public static int SecondsBetweenUserUpdateChecks { get { return secondsBetweenUserUpdateChecks; } private set { } }

        public static FasDataContext FasDataContext
        {
            get
            {
                if (fasDataContext == null)
                {
                    fasDataContext = new FasDataContext(DatabaseHelper.DBConnectionString);
                }

                return fasDataContext;
            }
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Works out how to create the initial database - if the database exists and we are not forceing a seed then this does nothing
        /// </summary>
        public static void CreateOrCopyTheSeedDatabase()
        {
            bool doSeed = false;

            // If the database doesnt exist, then either copy it from the project - or build it from seed data
            if (DatabaseHelper.FasDataContext.DatabaseExists() == false && !forceDatabaseSeed)
            {
                if (IsDbInProject())
                {
                    // if the db is attached to the project then copy it from the project to isolated storage
                    DatabaseHelper.CopyDatabaseToIsolatedStorage();
                }
                else
                {
                    // we want to seed the db from the json
                    doSeed = true;
                }
            }

            if (doSeed || forceDatabaseSeed)
            {
                DatabaseSeed databaseSeed = new DatabaseSeed();

                // Read the seed batch
                BatchUpdate batch = ReadJsonBatch(seedLocation);

                // if there is no db shipped with the xap then build it
                databaseSeed.CreateDatabaseFromInitialSeedData(DatabaseHelper.FasDataContext, batch);

                // Create a record in the database
                FillInSeedUpdateRecord("Seed", databaseSeedTime, batch);
            }
        }

        /// <summary>
        /// Is a database .sdf file shipped in the project directory
        /// </summary>
        private static bool IsDbInProject()
        {
            return Application.GetResourceStream(new Uri(projectDbLocation, UriKind.Relative)) != null;
        }

        /// <summary>
        /// Fill in the first seed update check record to the db
        /// </summary>
        private static void FillInSeedUpdateRecord(string type, DateTime date, BatchUpdate batch)
        {
            // Now fill in the update table
            UpdateCheckRecord checkRecord = new UpdateCheckRecord
            {
                Type = type,
                DateTime = date,
                Status = "Completed",
                ApiCountNeeded = 8,
                AppliedSuccessfully = true
            };

            ApiUpdateRecord record1 = new ApiUpdateRecord() { ApiName = "positions", DateTime = DateTime.Now, RecordCount = batch.Positions.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record2 = new ApiUpdateRecord() { ApiName = "clubs", DateTime = DateTime.Now, RecordCount = batch.Clubs.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record3 = new ApiUpdateRecord() { ApiName = "leagues", DateTime = DateTime.Now, RecordCount = batch.Leagues.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record4 = new ApiUpdateRecord() { ApiName = "countries", DateTime = DateTime.Now, RecordCount = batch.Countries.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record5 = new ApiUpdateRecord() { ApiName = "continents", DateTime = DateTime.Now, RecordCount = batch.Continents.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record6 = new ApiUpdateRecord() { ApiName = "players", DateTime = DateTime.Now, RecordCount = batch.Players.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record7 = new ApiUpdateRecord() { ApiName = "zones", DateTime = DateTime.Now, RecordCount = batch.Zones.Count, IsSuccess = true, UpdateCheck = checkRecord };
            ApiUpdateRecord record8 = new ApiUpdateRecord() { ApiName = "packages", DateTime = DateTime.Now, RecordCount = batch.Packages.Count, IsSuccess = true, UpdateCheck = checkRecord };

            checkRecord.Updates.Add(record1);
            checkRecord.Updates.Add(record2);
            checkRecord.Updates.Add(record3);
            checkRecord.Updates.Add(record4);
            checkRecord.Updates.Add(record5);
            checkRecord.Updates.Add(record6);
            checkRecord.Updates.Add(record7);
            checkRecord.Updates.Add(record8);

            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record1);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record2);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record3);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record4);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record5);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record6);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record7);
            DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record8);

            DatabaseHelper.FasDataContext.UpdateChecks.InsertOnSubmit(checkRecord);

            DatabaseHelper.FasDataContext.SubmitChanges();


            App.ViewModel.DbViewModel.AllUpdateChecks.Add(checkRecord);

            App.ViewModel.DbViewModel.AllApiUpdates.Add(record1);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record2);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record3);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record4);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record5);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record6);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record7);
            App.ViewModel.DbViewModel.AllApiUpdates.Add(record8);
        }


        /// <summary>
        /// Copy database from a project directory to isolated storage so we can use it for reads and writes
        /// </summary>
        private static void CopyDatabaseToIsolatedStorage()
        {
            // Obtain the virtual store for the application.
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

            // Create a stream for the file in the installation folder.
            using (Stream input = Application.GetResourceStream(new Uri(projectDbLocation, UriKind.Relative)).Stream)
            {
                // Create a stream for the new file in isolated storage.
                using (IsolatedStorageFileStream output = iso.CreateFile(isolatedStorageDbLocation))
                {
                    // Initialize the buffer.
                    byte[] readBuffer = new byte[4096];
                    int bytesRead = -1;

                    // Copy the file from the installation folder to isolated storage. 
                    while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        output.Write(readBuffer, 0, bytesRead);
                    }
                }
            }
        }


        /// <summary>
        /// Given a path, read all of the seed json
        /// </summary>
        private static BatchUpdate ReadJsonBatch(string path)
        {
            BatchUpdate batch = new BatchUpdate();

            string full = string.Format("/Zengo.WP8.FAS;component/{0}/", path);

            StreamResourceInfo playersSri = App.GetResourceStream(new Uri(full + "Players.json", UriKind.Relative));
            var players = JsonConvert.DeserializeObject<PlayersResponse>(new StreamReader(playersSri.Stream).ReadToEnd());

            StreamResourceInfo clubsSri = App.GetResourceStream(new Uri(full + "Clubs.json", UriKind.Relative));
            var clubs = JsonConvert.DeserializeObject<ClubsResponse>(new StreamReader(clubsSri.Stream).ReadToEnd());

            StreamResourceInfo leaguesSri = App.GetResourceStream(new Uri(full + "Leagues.json", UriKind.Relative));
            var leagues = JsonConvert.DeserializeObject<LeaguesResponse>(new StreamReader(leaguesSri.Stream).ReadToEnd());

            StreamResourceInfo countriesSri = App.GetResourceStream(new Uri(full + "Countries.json", UriKind.Relative));
            var countries = JsonConvert.DeserializeObject<CountriesResponse>(new StreamReader(countriesSri.Stream).ReadToEnd());

            StreamResourceInfo continentsSri = App.GetResourceStream(new Uri(full + "Continents.json", UriKind.Relative));
            var continents = JsonConvert.DeserializeObject<ContinentsResponse>(new StreamReader(continentsSri.Stream).ReadToEnd());

            StreamResourceInfo positionsSri = App.GetResourceStream(new Uri(full + "Positions.json", UriKind.Relative));
            var positions = JsonConvert.DeserializeObject<PositionsResponse>(new StreamReader(positionsSri.Stream).ReadToEnd());

            StreamResourceInfo zonesSri = App.GetResourceStream(new Uri(full + "Zones.json", UriKind.Relative));
            var zones = JsonConvert.DeserializeObject<ZonesResponse>(new StreamReader(zonesSri.Stream).ReadToEnd());

            StreamResourceInfo packagesSri = App.GetResourceStream(new Uri(full + "Packages.json", UriKind.Relative));
            var packages = JsonConvert.DeserializeObject<PackagesResponse>(new StreamReader(packagesSri.Stream).ReadToEnd());

            batch.Players = players.List;
            batch.Clubs = clubs.List;
            batch.Leagues = leagues.List;
            batch.Countries = countries.List;
            batch.Continents = continents.List;
            batch.Positions = positions.List;
            batch.Zones = zones.List;
            batch.Packages = packages.List;

            return batch;
        }

        #endregion
    }
}
