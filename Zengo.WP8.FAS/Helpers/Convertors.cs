
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public enum ConversionFunction { TrueToVisible, TrueToInvisible }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var param = Enum.Parse(typeof(ConversionFunction), (string)parameter, true);

            bool invert = ConversionFunction.TrueToInvisible.Equals((ConversionFunction)param);
            bool sourceValue = true.Equals(value);

            return ((sourceValue != invert)) ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isVisible = Visibility.Visible.Equals(value);
            bool isInverted = ConversionFunction.TrueToVisible.Equals(parameter);

            return ((isInverted && !isVisible) || (!isInverted && isVisible));
        }
    }
}
