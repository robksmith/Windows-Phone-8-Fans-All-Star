
#region Usings

using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion


namespace Microsoft.Phone.Shell 
{
    public static class AppBarHelper
    {
        public static void Enable(this IApplicationBar appBar)
        {
            if (appBar == null) return;

            appBar.IsMenuEnabled = true;

            foreach (var obj in appBar.Buttons)
            {
                var button = obj as ApplicationBarIconButton;
                if (button != null)
                {
                    button.IsEnabled = true;
                }
            }
        }

        public static void Disable(this IApplicationBar appBar)
        {
            if (appBar == null) return;

            appBar.IsMenuEnabled = false;

            foreach (var obj in appBar.Buttons)
            {
                var button = obj as ApplicationBarIconButton;
                if (button != null)
                {
                    button.IsEnabled = false;
                }
            }
        }
    } 
}
