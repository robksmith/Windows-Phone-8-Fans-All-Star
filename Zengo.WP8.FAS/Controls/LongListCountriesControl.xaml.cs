
#region Usings

using Zengo.WP8.FAS.Models;
using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class LongListCountriesControl : UserControl
    {
        public class CountrySelectionChangedEventArgs : EventArgs
        {
            public CountryRecord Country { get; set; }
        }


        #region Events

        public event EventHandler<CountrySelectionChangedEventArgs> SelectionChanged;

        #endregion


        #region Properties

        private bool forAccountPage;
        public bool ForAccountPage 
        {
            get { return forAccountPage; }
            set
            {
                forAccountPage = value;
                if (forAccountPage)
                {
                    countriesLongList.ItemTemplate = (DataTemplate)Resources["countriesItemForAccountTemplate"];
                }
                else
                {
                    countriesLongList.ItemTemplate = (DataTemplate)Resources["countriesItemForSearchTemplate"];
                }
            } 
        }

        #endregion


        #region Constructors

        public LongListCountriesControl()
        {
            InitializeComponent();

            countriesLongList.SelectionChanged += new SelectionChangedEventHandler(countriesLongList_SelectionChanged);
        }

        #endregion


        #region Events

        void countriesLongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new CountrySelectionChangedEventArgs() { Country = (CountryRecord)countriesLongList.SelectedItem });
            }
        }

        #endregion
    }
}
