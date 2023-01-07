using System;
using System.Globalization;
using System.Windows.Media;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class MessageUrgencyToBrushConverter : BaseValueConverter<MessageUrgencyToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Unpack the MessageMetadata into its two variables, sendDate and isTeacher
            string date = value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
            bool isTeacher = System.Convert.ToBoolean(value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);

            // If the message was not sent by a teacher (sent by a student)...
            if (!isTeacher)
            {
                // Count the number of days since the message was sent, capped at 10 days
                int daysElapsed = Math.Min(DateTime.Now.DayOfYear - DateTime.Parse(date).DayOfYear, 10);

                // Increase red as the message gets older, green as it gets younger (see Desmos configuration)
                int R = System.Convert.ToInt32(Math.Round(-2.2 * ((daysElapsed - 10) * (daysElapsed - 10)) + 220));
                int G = System.Convert.ToInt32(Math.Round(-2.2 * daysElapsed * daysElapsed + 220));

                // Return the appropriate solid colour brush
                return new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, 30));
            }
            // Otherwise, colour it blue
            else
            {
                return new SolidColorBrush(Color.FromArgb(255, 68, 100, 193));
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
