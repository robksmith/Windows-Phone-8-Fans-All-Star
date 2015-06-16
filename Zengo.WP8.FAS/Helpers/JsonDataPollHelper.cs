
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class BatchUpdate
    {
        public List<Player> Players { get; set; }
        public List<Club> Clubs { get; set; }
        public List<League> Leagues { get; set; }
        public List<Continent> Continents { get; set; }
        public List<Country> Countries { get; set; }
        public List<Position> Positions { get; set; }
        public List<Zone> Zones { get; set; }
        public List<Package> Packages { get; set; }


        public bool ShowApplyingUpdates()
        {
            if (TotalRecords() >= 50)
            {
                return true;
            }

            return false;
        }


        public int TotalRecords()
        {
            int count = 0;

            if (Players != null) count += Players.Count;
            if (Clubs != null) count += Clubs.Count;
            if (Leagues != null) count += Leagues.Count;
            if (Continents != null) count += Continents.Count;
            if (Countries != null) count += Countries.Count;
            if (Positions != null) count += Positions.Count;
            if (Zones != null) count += Zones.Count;
            if (Packages != null) count += Packages.Count;

            return count;
        }
    }

    public class JsonDataPollHelper
    {
        #region Inner Classes

        class ThreadParams
        {
            public BatchUpdate Batch { get; set; }
            public int ApisNeeded { get; set; }

            public UpdateCheckRecord CheckRecord { get; set; }
        }

        #endregion


        #region Fields

        // Timer for the update api
        DispatcherTimer updateTimer = null;

        List<UpdateCheck> checks;

        #endregion


        #region Constructors

        public JsonDataPollHelper()
        {
            checks = new List<UpdateCheck>();
        }

        #endregion


        #region Initialisers

        /// <summary>
        /// Set off our timer to check for json data updates
        /// </summary>
        public void Start()
        {
            if (updateTimer == null)
            {
                if (App.AppConstants.ShowGameDataPollMessages)
                {
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "Performing first data update check - please wait" }));
                }

                updateTimer = new System.Windows.Threading.DispatcherTimer();
                updateTimer.Interval = new TimeSpan(0, 0, DatabaseHelper.SecondsBetweenDataUpdateChecks);
                updateTimer.Tick += new EventHandler(DataTimerEndTick);
                updateTimer.Start();

                // The first time, we probably need to invoke a check here
                FetchUpdateBatch();
            }
        }

        #endregion


        #region Data Update Methods

        void FetchUpdateBatch()
        {
            // Get the last successful update 
            UpdateCheckRecord lastUpdate = App.ViewModel.DbViewModel.LastSuccessfullUpdate(); 
            DateTime lastSuccessfullUpdate = lastUpdate.DateTime;

            // cancel any existing ones
            foreach (UpdateCheck existingCheck in checks)
            {
                existingCheck.Cancel = true;
            }
            checks.Clear();

            // Create a new check
            UpdateCheck check = new UpdateCheck(this, lastSuccessfullUpdate);
            checks.Add(check);

            // Go
            check.Start();
        }


        /// <summary>
        /// Called from the update check when its done
        /// </summary>
        /// <param name="batch"></param>
        public void BatchFinishedDownloading(BatchUpdate batch, int neededApis, UpdateCheckRecord checkRecord)
        {
            // Show a modal popup on the screen to tell them what is happening - because this cant be done in the thread
            if (batch.ShowApplyingUpdates())
            {
                // Disable the app bar
                var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
                if (currentPage is PhoneApplicationPage)
                {
                    (currentPage as PhoneApplicationPage).ApplicationBar.Disable();
                }

                // Show the 'applying updates' popup
                App.PopupApplyingUpdatesHelper.Show();
            }

            // If updates are being applied we can't do certain things
            App.AppConstants.ApplyingUpdates = true;

            // Update the database in a new thread
            ThreadParams tp = new ThreadParams { Batch = batch, ApisNeeded = neededApis, CheckRecord = checkRecord };

            // Start the update thread
            Process(tp);
        }

        /// <summary>
        /// The update check has completed with no changes required
        /// </summary>
        public void CompletedNoChangesRequired()
        {
            if (App.AppConstants.ShowGameDataPollMessages)
            {
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = string.Format("Data update completed : 0 apis required") }, TimeSpan.FromSeconds(1.0)));
            }
        }


        void Process(ThreadParams tp)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

            worker.DoWork += (s, e) =>
            {
                DatabaseUpdate databaseUpdate = new DatabaseUpdate();

                // At this now we have the database, apply the updates
                databaseUpdate.ApplyUpdates(DatabaseHelper.FasDataContext, tp.Batch, worker);

                // Set the applied successfully flag
                tp.CheckRecord.AppliedSuccessfully = true;
                DatabaseHelper.FasDataContext.SubmitChanges();
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
#if DEBUG
                if (e.Error != null)
                {
                    if (e.Error.InnerException != null)
                    {
                        MessageBox.Show(e.Error.InnerException.Message, e.Error.Message, MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show(e.Error.Message);
                    }
                }
#endif
                // Enable the app bar
                var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
                if (currentPage is PhoneApplicationPage)
                {
                    (currentPage as PhoneApplicationPage).ApplicationBar.Enable();
                }

                // Updates are done
                App.AppConstants.ApplyingUpdates = false;

                // Show a debug message
                if (App.AppConstants.ShowGameDataPollMessages)
                {
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = string.Format("Data update completed : {0} apis updated", tp.ApisNeeded) }, TimeSpan.FromSeconds(1.0)));
                }

                // If we are on the main page then update the icons
                App.ViewModel.DbViewModel.RefreshMainPage();

                // Remove the modal popup "applying updates" 
                App.PopupApplyingUpdatesHelper.Hide();
            };

            // Run the async process
            worker.RunWorkerAsync();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            App.PopupApplyingUpdatesHelper.SetProgress(e.ProgressPercentage);
        }

        /// <summary>
        /// Our database updates are done in a separate thread
        /// </summary>
        /// <param name="data"></param>
        //private void ApplyUpdates(object data)
        //{
        //    ThreadParams threadParams = (ThreadParams)data;

        //    //App.RootFrame.Dispatcher.BeginInvoke(delegate()
        //    //{
        //        DatabaseUpdate databaseUpdate = new DatabaseUpdate();

        //        // At this now we have the database, apply the updates
        //        databaseUpdate.ApplyUpdates(DatabaseHelper.FasDataContext, threadParams.Batch);

        //        // Set the applied successfully flag
        //        threadParams.CheckRecord.AppliedSuccessfully = true;
        //        DatabaseHelper.FasDataContext.SubmitChanges();

        //    //});
        //}

        #endregion


        #region Timer Events

        /// <summary>
        /// Called every x seconds to check for data updates
        /// </summary>
        void DataTimerEndTick(object sender, EventArgs e)
        {
            if (DatabaseHelper.FireUpdates)
            {
                if (App.AppConstants.ShowGameDataPollMessages)
                {
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = "Data update starting" }, TimeSpan.FromSeconds(1.0)));
                }

                // Start the updates
                FetchUpdateBatch();
            }
        }

        #endregion
    }

}
