using System;
using System.Globalization;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class TitleLengthToFontSizeConverter : BaseValueConverter<TitleLengthToFontSizeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int length = System.Convert.ToString(value).Length;

            return length > 40 ? Math.Max(80 - length, 10) : 30;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
