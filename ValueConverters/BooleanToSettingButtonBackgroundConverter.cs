using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class BooleanToSettingButtonBackgroundConverter : BaseValueConverter<BooleanToSettingButtonBackgroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return new BrushConverter().ConvertFrom("#49bd26");
            }
            else
            {
                return new BrushConverter().ConvertFrom("#eb2421");
            }

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
