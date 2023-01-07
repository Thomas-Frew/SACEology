using System;
using System.Globalization;
using System.Windows.Media;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class RecordingButtonStateToColourConverter : BaseValueConverter<RecordingButtonStateToColourConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((RecordingButtonState)value)
            {
                case RecordingButtonState.Stop:
                    return (SolidColorBrush)(new BrushConverter().ConvertFrom("#eb2421"));

                default:
                    return (SolidColorBrush)(new BrushConverter().ConvertFrom("#151515"));
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
