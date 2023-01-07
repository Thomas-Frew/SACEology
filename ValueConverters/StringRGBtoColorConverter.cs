using System;
using System.Globalization;
using System.Windows.Media;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class StringRGBToColorConverter : BaseValueConverter<StringRGBToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Returns the appropriate colour
            return (Color)(new ColorConverter().ConvertFrom(value));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
