
#region Usings

using Zengo.WP8.FAS.Models;
using System.Windows.Controls;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS.Controls
{
    public partial class PagePlayerListHeaderControl : UserControl
    {
        public string PageTitle { get { return TextboxPageTitle.Text; } set { TextboxPageTitle.Text = value; } }
        public string PageName { get { return TextboxPageName.Text; } set { TextboxPageName.Text = value; } }

        public PagePlayerListHeaderControl()
        {
            InitializeComponent();

            //EnableProgresBar(false);
        }

        //internal void EnableProgresBar(bool enabled)
        //{
        //    progressBar.Visibility = enabled == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        //}

        internal void Setup(ZoneRecord zone, ClubRecord club, CountryRecord country, bool? isEurope)
        {
            if (club != null)
            {
                TextboxSearchByHeading.Text = AppResources.ListingByClub;
                TextboxSearchByCriteria.Text = club.Name;
                TextboxSearchByEurope.Text = LocationText(isEurope);
            }
            else if (zone != null)
            {
                TextboxSearchByHeading.Text = AppResources.ListingByPosition;
                TextboxSearchByCriteria.Text = zone.Name;
                TextboxSearchByEurope.Text = LocationText(isEurope); ;
            }
            else if (country != null)
            {
                TextboxSearchByHeading.Text = AppResources.ListingByCountry;
                TextboxSearchByCriteria.Text = country.Name;
                TextboxSearchByEurope.Text = LocationText(isEurope); ;
            }
        }

        private string LocationText(bool? isEurope)
        {
            if (isEurope==null) 
            {
                TextboxSearchByFrom.Visibility = System.Windows.Visibility.Collapsed;
                TextboxSearchByEurope.Visibility = System.Windows.Visibility.Collapsed;
                return "";//Europe & Rest of the world";
            }
            else 
            {
                TextboxSearchByFrom.Visibility = System.Windows.Visibility.Visible;
                TextboxSearchByEurope.Visibility = System.Windows.Visibility.Visible;
                return isEurope == true ? AppResources.EuropeName : AppResources.ROTWName;
            }
        }
    }
}
