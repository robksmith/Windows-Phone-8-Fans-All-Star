using Zengo.WP8.FAS.Resources;

namespace Zengo.WP8.FAS
{
    public partial class LanguagePage
    {
        public LanguagePage()
        {
            InitializeComponent();
            PageHeaderControl.PageName = AppResources.AppBarLanguage;
            PageHeaderControl.PageTitle = AppResources.ProductTitle;
        }
    }
}