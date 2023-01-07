using System;
using System.Globalization;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class MessagesToDisplayMessage : BaseValueConverter<MessagesToDisplayMessage>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToInt16(value) == 1)
            {
                return System.Convert.ToString(value) + " New Message";
            }
            else
            {
                return System.Convert.ToString(value) + " New Messages";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
