using System;
using System.Globalization;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class MessageLengthToFontSizeConverter : BaseValueConverter<MessageLengthToFontSizeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int length = System.Convert.ToString(value).Length;

            return length > 35 ? Math.Max(57 - length, 10) : 22;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
