using System;
using System.Globalization;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class RecordingButtonStateToIconConverter : BaseValueConverter<RecordingButtonStateToIconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((RecordingButtonState)value)
            {
                case RecordingButtonState.Record:
                    return "\uf192";

                case RecordingButtonState.Stop:
                    return "\uf28d";

                case RecordingButtonState.Play:
                    return "\uf144";

                case RecordingButtonState.Pause:
                    return "\uf28b";

                default:
                    return "\uf192";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
