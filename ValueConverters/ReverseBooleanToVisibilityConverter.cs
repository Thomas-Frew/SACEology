using System;
using System.Globalization;
using System.Windows;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class ReserveBooleanToVisibilityConverter : BaseValueConverter<ReserveBooleanToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool passedValue;

            int numericValue;
            bool isNumeric = int.TryParse(value.ToString(), out numericValue);

            if (isNumeric)
            {
                passedValue = numericValue == 1;
            }
            else
            {
                passedValue = (bool)value;
            }

            if (!passedValue)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
