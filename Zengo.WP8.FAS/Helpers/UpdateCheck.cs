
#region Usings

using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using Zengo.WP8.FAS.WepApi.Api;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Zengo.WP8.FAS.WepApi;
using System.Globalization;

#endregion


namespace Zengo.WP8.FAS.Helpers
{
    public class UpdateCheck
    {
        #region Fields

        DataApi game;
        BatchUpdate batch;

        JsonDataPollHelper updateHelper;
        DateTime lastSuccessfullUpdate;

        UpdateCheckRecord checkRecord;

        // This is the number of apis we need to download
        int apisNeeded = 0;

        // this is the amount of ok responses needed
        int responseCount = 0;

        #endregion


        public bool Cancel { get; set; }


        #region Constructors

        public UpdateCheck(JsonDataPollHelper updateHelper, DateTime lastSuccessfullUpdate)
        {
            Cancel = false;

            game = new DataApi();
            batch = new BatchUpdate();

            this.lastSuccessfullUpdate = lastSuccessfullUpdate;
            this.updateHelper = updateHelper;

            // Set up the api callbacks
            game.LastModifiedCompleted += game_LastModifiedCompleted;

            game.PlayersCompleted += players_RetrieveCompleted;
            game.ClubsCompleted += clubs_RetrieveCompleted;
            game.ContinentsCompleted += game_ContinentsCompleted;
            game.PositionsCompleted += game_PositionsCompleted;
            game.CountriesCompleted += game_CountriesCompleted;
            game.LeaguesCompleted += game_LeaguesCompleted;
            game.ZonesCompleted += game_ZonesCompleted;
            game.PackagesCompleted += game_PackagesCompleted;
        }

        #endregion


        #region Helpers

        /// <summary>
        /// The update helper calls this start method which starts to query the web service
        /// </summary>
        public void Start()
        {
            // Now fill in the update table
            checkRecord = new UpdateCheckRecord
            {
                Type = "Update",
                DateTime = DateTime.Now,
                Status = "Getting Modified List",
                ApiCountNeeded = null,
                AppliedSuccessfully = false
            };

            App.ViewModel.DbViewModel.AllUpdateChecks.Add(checkRecord);

            DatabaseHelper.FasDataContext.UpdateChecks.InsertOnSubmit(checkRecord);
            DatabaseHelper.FasDataContext.SubmitChanges();

            // Make the api calls
            game.LastModified();
        }

        /// <summary>
        /// Called after each successful api download - when all required apis are downloaded, we can then process them
        /// </summary>
        private void CountSuccessfulResponses()
        {
            responseCount += 1;

            if (responseCount == apisNeeded)
            {
                // Update the check record
                checkRecord.Status = "All Fetched OK";
                checkRecord.ApiCountNeeded = apisNeeded;
                DatabaseHelper.FasDataContext.SubmitChanges();

                // call back to the updatehelper with our downloaded batch - ready to update the db
                updateHelper.BatchFinishedDownloading(batch, apisNeeded, checkRecord);
            }
        }

        #endregion


        #region The last modified (update) response

        /// <summary>
        /// We have received the modified response - we now know what we api call we are required to make
        /// </summary>
        void game_LastModifiedCompleted(object sender, LastModifiedEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                bool needPlayers = IsDataAvailable(e, "players");
                bool needPositions = IsDataAvailable(e, "positions");
                bool needClubs = IsDataAvailable(e, "clubs");
                bool needLeagues = IsDataAvailable(e, "leagues");
                bool needcountries = IsDataAvailable(e, "countries");
                bool needContinents = IsDataAvailable(e, "continents");
                bool needZones = IsDataAvailable(e, "zones");
                bool needPackages = IsDataAvailable(e, "packages");

                responseCount = 0;
                apisNeeded = 0;

                if (needPlayers)
                {
                    apisNeeded += 1;
                    game.Players(lastSuccessfullUpdate);
                }

                if (needClubs)
                {
                    apisNeeded += 1;
                    game.Clubs(lastSuccessfullUpdate);
                }

                if (needContinents)
                {
                    apisNeeded += 1;
                    game.Continents(lastSuccessfullUpdate);
                }

                if (needPositions)
                {
                    apisNeeded += 1;
                    game.Positions(lastSuccessfullUpdate);
                }

                if (needcountries)
                {
                    apisNeeded += 1;
                    game.Countries(lastSuccessfullUpdate);
                }

                if (needLeagues)
                {
                    apisNeeded += 1;
                    game.Leagues(lastSuccessfullUpdate);
                }

                if (needZones)
                {
                    apisNeeded += 1;
                    game.Zones(lastSuccessfullUpdate);
                }

                if (needPackages)
                {
                    apisNeeded += 1;
                    game.Packages(lastSuccessfullUpdate);
                }

                if (apisNeeded == 0)
                {
                    // Update the check record
                    checkRecord.Status = "Nothing To Update";
                    checkRecord.ApiCountNeeded = apisNeeded;
                    DatabaseHelper.FasDataContext.SubmitChanges();

                    updateHelper.CompletedNoChangesRequired();
                }
                else
                {
                    // Update the check record
                    checkRecord.Status = "Fetching Updates";
                    checkRecord.ApiCountNeeded = apisNeeded;
                    DatabaseHelper.FasDataContext.SubmitChanges();
                }
            }
            else
            {
                // Update the check record
                checkRecord.Status = "Modified Fetch Failed";
                checkRecord.ApiCountNeeded = null;
                DatabaseHelper.FasDataContext.SubmitChanges();

                Cancel = true;
            }
        }

        /// <summary>
        /// Given the last modified json and an api name, lets us know if more data is available
        /// </summary>
        private bool IsDataAvailable(LastModifiedEventArgs e, string api)
        {
            if (e.ServerResponse.Details.List != null)
            {
                var lastModified = (from lm in e.ServerResponse.Details.List where lm.Api == api select lm).FirstOrDefault();
                if (lastModified != null && DateTime.Parse(lastModified.Time, CultureInfo.InvariantCulture) >= lastSuccessfullUpdate)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region Response received events

        void game_PackagesCompleted(object sender, PackagesEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Packages = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "packages", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void game_ZonesCompleted(object sender, ZonesEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Zones = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "zones", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void game_LeaguesCompleted(object sender, LeaguesEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Leagues = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "leagues", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void game_CountriesCompleted(object sender, CountriesEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Countries = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "countries", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void players_RetrieveCompleted(object sender, PlayersEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Players = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "players", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void clubs_RetrieveCompleted(object sender, ClubsEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Clubs = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "clubs", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void game_ContinentsCompleted(object sender, ContinentsEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Continents = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "continents", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        void game_PositionsCompleted(object sender, PositionsEventArgs e)
        {
            if (Cancel)
            {
                return;
            }

            if (e.ConnectionError == ApiConnectionResult.Good)
            {
                batch.Positions = e.ServerResponse.List;

                ApiUpdateRecord record = new ApiUpdateRecord() { ApiName = "positions", DateTime = DateTime.Now, IsSuccess = true, RecordCount = e.ServerResponse.List.Count, UpdateCheck = checkRecord };

                App.ViewModel.DbViewModel.AllApiUpdates.Add(record);

                DatabaseHelper.FasDataContext.ApiUpdates.InsertOnSubmit(record);
                DatabaseHelper.FasDataContext.SubmitChanges();

                checkRecord.Updates.Add(record);

                CountSuccessfulResponses();
            }
            else
            {
                Cancel = true;
            }
        }

        #endregion

    }
}
