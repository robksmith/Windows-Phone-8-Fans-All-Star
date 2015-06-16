
#region Usings

using Zengo.WP8.FAS.Helpers;
using System.Windows.Controls;

#endregion

namespace Zengo.WP8.FAS.Controls.ListItems
{
    public partial class CountryItemForSearch : UserControl
    {
        #region Constructors

        public CountryItemForSearch()
        {
            InitializeComponent();

            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }

        #endregion

    }
}
