
#region Usings

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Zengo.WP8.FAS.Controls;
using System;
using System.Windows.Controls.Primitives;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class PopupApplyingUpdatesHelper
    {
        #region Fields

        Popup popupControl;
        PopupAppyingUpdatesControl messageControl;

        #endregion


        #region Properties


        #endregion


        #region Constructors

        public PopupApplyingUpdatesHelper()
        {
            // Create the popups
            popupControl = new Popup();
            messageControl = new PopupAppyingUpdatesControl();

            popupControl.Child = messageControl;
            popupControl.VerticalOffset = 28;
        }

        #endregion


        internal void Show()
        {
            //try
            //{
            //    var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
            //    if (currentPage is PhoneApplicationPage)
            //    {
            //        (currentPage as PhoneApplicationPage).ApplicationBar.IsVisible = false;
            //    }
            //}
            //catch (Exception)
            //{
            //}

            popupControl.IsOpen = true;
        }

        internal void Hide()
        {
            popupControl.IsOpen = false;

            //try
            //{
            //    var currentPage = ((PhoneApplicationFrame)App.Current.RootVisual).Content;
            //    if (currentPage is PhoneApplicationPage)
            //    {
            //        (currentPage as PhoneApplicationPage).ApplicationBar.IsVisible = true;
            //    }
            //}
            //catch (Exception)
            //{
            //}
        }

        internal void SetProgress(int percent)
        {
            messageControl.SetProgress(percent);
        }
    }
}
