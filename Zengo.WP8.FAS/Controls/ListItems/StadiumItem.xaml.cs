using System.Windows.Controls;
using Zengo.WP8.FAS.Helpers;

namespace Zengo.WP8.FAS.Controls.ListItems
{
    public partial class StadiumItem
    {
        public StadiumItem()
        {
            InitializeComponent();
            LayoutRoot.ManipulationStarted += Animation.Standard_ManipulationStarted_1;
            LayoutRoot.ManipulationCompleted += Animation.Standard_ManipulationCompleted_1;
        }
    }
}
