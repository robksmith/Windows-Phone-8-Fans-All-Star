
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

#endregion


namespace Zengo.WP8.FAS.Helpers
{
    public static class Animation
    {

        #region Animation

        public static void Standard_ManipulationStarted_1(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = new System.Windows.Media.TranslateTransform() { X = 0, Y = -8 };
        }

        public static void Standard_ManipulationCompleted_1(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            ((UIElement)sender).RenderTransform = null;
        }

        #endregion
    }
}
