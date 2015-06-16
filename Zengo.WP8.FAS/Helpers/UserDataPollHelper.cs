
#region Usings

using Zengo.WP8.FAS.Controls;
using Zengo.WP8.FAS.Resources;
using Zengo.WP8.FAS.WepApi;
using System;
using System.Windows.Threading;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class UserDataPollHelper
    {
        #region Fields

        // Timer for the user api sync updates (votes, user, transactions)
        DispatcherTimer userTimer = null;

        // To poll the transactions and votes api
        UserApi userApi;

        int totalFetches;

        // How many times do we want to check votes
        int maxVoteAttempts;
        
        // How many times do we want to check transactions
        int maxTransactionAttempts;

        //// New votes have been found by the polling mechanism
        //private bool newVotesFound = false;

        //// New transactions have been found by the polling mechanism
        //private bool newTansactionsFound = false;

        #endregion


        #region Constructors

        public UserDataPollHelper()
        {
            userApi = new UserApi();

            userApi.PitchHistoryCompleted += userApi_PitchHistoryCompleted;
            userApi.TransactionsCompleted += userApi_TransactionsCompleted;
            //userApi.VotesCompleted += userApi_VotesCompleted;
            userApi.VotesAvailableCompleted += userApi_VotesAvailableCompleted;
            userApi.MovingCompleted += userApi_MovingCompleted;
            userApi.ValidatedCompleted += userApi_ValidatedCompleted;
        }

        #endregion


        #region Initialisers

        /// <summary>
        /// set off our timer to check for transaction updates
        /// </summary>
        public void Start(int maxVoteAttempts, int maxTransactionAttempts)
        {
            // Just in case its already started
            //Stop();

            ResetVoteAttempts(maxVoteAttempts);
            ResetTransactionAttempts(maxTransactionAttempts);

            // Start a timer to retrieve user details
            if (userTimer == null)
            {
                if (App.AppConstants.ShowUserDataPollMessages)
                {
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = "Initialising user sync checks" }));
                }

                userTimer = new System.Windows.Threading.DispatcherTimer();
                userTimer.Interval = new TimeSpan(0, 0, DatabaseHelper.SecondsBetweenUserUpdateChecks);
                userTimer.Tick += new EventHandler(UserTimerEndTick);
                userTimer.Start();

                // The initial fetch
                FetchUserData();
            }
        }

        //public void Stop()
        //{
        //    // stop the timer
        //    if (userTimer != null)
        //    {
        //        if (App.AppConstants.ShowUserDataPollMessages)
        //        {
        //            App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = "Stopping user sync checks" }));
        //        }

        //        userTimer.Stop();
        //        userTimer.Tick -= new EventHandler(UserTimerEndTick);
        //        userTimer = null;
        //    }
        //}

        public void ResetTransactionAttempts(int maxTransactionAttempts)
        {
            this.maxTransactionAttempts = maxTransactionAttempts;
            //newTansactionsFound = false;
        }

        public void ResetVoteAttempts(int maxVoteAttempts)
        {
            this.maxVoteAttempts = maxVoteAttempts;
            //newVotesFound = false;
        }

        //public void CancelTransactionAttempts()
        //{
        //    this.maxTransactionAttempts = 0;
        //}

        //public void CancelVoteAttempts()
        //{
        //    this.maxVoteAttempts = 0;
        //}

        #endregion


        #region Data Update Methods

        /// <summary>
        /// Called the first time someone visits the start page - maybe also on the timer tick
        /// </summary>
        void FetchUserData()
        {
            if (App.AppConstants.ShowUserDataPollMessages)
            {
                App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageDebugControl() { Message = "Performing user sync check" }));
            }

            // Only get the player modified values every say 30 minutes
            if (totalFetches % 30 == 0)
            {
                userApi.MovingQuery();
            }

            // Get new votes and transactions - and check the validated flag
            if (App.ViewModel.DbViewModel.IsLoggedOn())
            {
                //userApi.UserQuery(App.ViewModel.DbViewModel.CurrentUser.UserId);

                // Check votes for the sync and merge
                if (maxVoteAttempts > 0)
                {
                    //userApi.VotesAllQuery(App.ViewModel.DbViewModel.CurrentUser.UserId);
                    userApi.VotesAvailableQuery(App.ViewModel.DbViewModel.CurrentUser.UserId, App.ViewModel.DbViewModel.CurrentUser.LastAvailableVoteHistoryRetrievalTime);
                    maxVoteAttempts -= 1;
                }

                // Get the pitch history
                userApi.PitchHistory(App.ViewModel.DbViewModel.CurrentUser.UserId);

                // Check transactions
                if (maxTransactionAttempts > 0)
                {
                    userApi.TransactionQuery(App.ViewModel.DbViewModel.CurrentUser.UserId);
                    maxTransactionAttempts -= 1;
                }

                // If the user isn't validated, then check it
                if (!App.ViewModel.DbViewModel.CurrentUser.IsValidated)
                {
                    userApi.ValidatedQuery(App.ViewModel.DbViewModel.CurrentUser.UserId);
                }
            }

            // Keep a count
            totalFetches += 1;
        }

        #endregion


        #region Timer Events

        /// <summary>
        /// Called every x seconds to check for transaction and vote changes
        /// </summary>
        void UserTimerEndTick(object sender, EventArgs e)
        {
            // sync user data
            FetchUserData();
        }

        #endregion


        #region User Api Event Handlers

        /// <summary>
        /// Votes api has returned - so merge in any new votes
        /// </summary>
        void userApi_VotesAvailableCompleted(object sender, VotesAvailableEventArgs e)
        {
            if (!App.ViewModel.DbViewModel.IsLoggedOn())
            {
                return;
            }

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // Check if they have actually made a purchase
                //if (App.ViewModel.DbViewModel.VotesRequireSyncing(e.ServerResponse.Details.Votes))

                TimeSpan dif = DateTime.Now - App.ViewModel.DbViewModel.CurrentUser.LastLocalCastTime;

                // Only if more than 20 seconds have elapsed since the last cast vote response ( and subsequent merge ) do we auto merge the available votes
                if ( dif.TotalSeconds > 30 )
                {
                    // If they have, update the database with the new votes
                    bool merged = App.ViewModel.DbViewModel.MergeVotes(e.ServerResponse);

                    // show a message if votes have been merged
                    if (merged)
                    {
                        App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = AppResources.VotesSynced }, new TimeSpan(0, 0, 1)));
                    }

                    // Now merge the new pitch data returned with this result
                    App.ViewModel.DbViewModel.UpdateCurrentPitch(e.ServerResponse.Details.Positions);
                }
            }
        }

        /// <summary>
        /// Moving api has returned
        /// </summary>
        void userApi_MovingCompleted(object sender, MovingEventArgs e)
        {
            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // If they have, update the database with the new moving values
                bool merged = App.ViewModel.DbViewModel.MergeMoving(e.ServerResponse);

                // show a message once moving is merged
                if (merged)
                {
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = AppResources.PlayerMoveSynced }, new TimeSpan(0, 0, 1)));
                }
            }
        }


        /// <summary>
        /// The pitch history api has returned
        /// </summary>
        void userApi_PitchHistoryCompleted(object sender, PitchHistoryEventArgs e)
        {
            if (!App.ViewModel.DbViewModel.IsLoggedOn())
            {
                return;
            }

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // Check if these pitches need merging
                if (App.ViewModel.DbViewModel.PitchesRequireSyncing(e.ServerResponse))
                {
                    // If they have, update the database with the new transactions
                    App.ViewModel.DbViewModel.UpdateHistoricalPitches(e.ServerResponse);

                    // show a message
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = "Your pitch history is synced" }, new TimeSpan(0, 0, 1)));
                }
            }
        }


        /// <summary>
        /// The transaction api has returned
        /// </summary>
        void userApi_TransactionsCompleted(object sender, TransactionsEventArgs e)
        {
            if (!App.ViewModel.DbViewModel.IsLoggedOn())
            {
                return;
            }

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // Check if they have actually made a purchase
                if (App.ViewModel.DbViewModel.TransactionsRequireSyncing(e.ServerResponse))
                {
                    //CancelTransactionAttempts();
                    //if (!newTansactionsFound)
                    //{
                    //    // set this so we dont display message again
                    //    newTansactionsFound = true;

                        // If they have, update the database with the new transactions
                        App.ViewModel.DbViewModel.UpdateTransactions(e.ServerResponse);

                        // show a message
                        App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = AppResources.TransactionSynced }, new TimeSpan(0, 0, 1)));
                    //}
                }
            }
        }


        void userApi_ValidatedCompleted(object sender, ValidatedEventArgs e)
        {
            if (!App.ViewModel.DbViewModel.IsLoggedOn())
            {
                return;
            }

            if (e.ConnectionError == WebApi.Responses.ApiConnectionResult.Good)
            {
                // Check if the validation result is different to the current users validation
                if (App.ViewModel.DbViewModel.CompareValidated(e.ServerResponse))
                {
                    // If it is, update the users validated flag
                    App.ViewModel.DbViewModel.UpdateValidated(e.ServerResponse);

                    // show a message
                    App.PopupHelper.PopupMessages.Enqueue(new PopupMessage(new PopupMessageControl() { Message = AppResources.AccountValidated }, new TimeSpan(0, 0, 1)));
                }
            }
        }

        #endregion
    }

}
