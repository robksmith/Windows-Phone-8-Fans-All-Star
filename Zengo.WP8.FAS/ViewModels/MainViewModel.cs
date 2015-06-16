
#region Usings

using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.ViewModels;
using System;
using System.ComponentModel;

#endregion


namespace Zengo.WP8.FAS
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        DbViewModel dbViewModel;

        #endregion


        #region Properties

        /// <summary>
        /// The database view model
        /// </summary>
        public DbViewModel DbViewModel
        {
            get
            {
                return dbViewModel;
            }
            set
            {
                dbViewModel = value;
                NotifyPropertyChanged("DbViewModel");
            }
        }

        // Show a small number on the player icon specifying the number of votes per position
        public bool ShowVotesPerPosition { get; set; }

        public bool ShowMyTeam { get; set; }

        #endregion


        #region Constructors

        public MainViewModel()
        {
            // Create the ViewModel object.
            dbViewModel = new DbViewModel();

            // Query the local database and load observable collections.
            dbViewModel.LoadCollectionsFromDatabase();

            ShowMyTeam = true;
            ShowVotesPerPosition = false;
        }

        #endregion


        #region Methods

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}