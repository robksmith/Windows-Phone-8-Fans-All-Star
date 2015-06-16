
#region Usings

using Zengo.WP8.FAS.Controls.ListItems;
using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace Zengo.WP8.FAS.Controls
{

    public partial class LongListClubsControl : UserControl
    {
        public class ClubSelectionChangedEventArgs : EventArgs
        {
            public ClubRecord Club { get; set; }
        }


        #region Events

        public event EventHandler<ClubSelectionChangedEventArgs> SelectionChanged;

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
                    clubsLongList.ItemTemplate = (DataTemplate)Resources["clubItemForAccountTemplate"];
                }
                else
                {
                    clubsLongList.ItemTemplate = (DataTemplate)Resources["clubItemForSearchTemplate"];
                }
            }
        }

        #endregion


        #region Constructors

        public LongListClubsControl()
        {
            InitializeComponent();

            clubsLongList.SelectionChanged += new SelectionChangedEventHandler(categoriesLongList_SelectionChanged);
        }

        #endregion



        #region Events

        void categoriesLongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new ClubSelectionChangedEventArgs() { Club = (ClubRecord)clubsLongList.SelectedItem });
            }
        }


        #endregion
    }
}
