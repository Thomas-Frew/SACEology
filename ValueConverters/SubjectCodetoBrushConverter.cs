using System;
using System.Globalization;
using System.Windows.Media;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class SubjectCodeToBrushConverter : BaseValueConverter<SubjectCodeToBrushConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the angular positions of the performance standard
            char firstCharacter = System.Convert.ToChar(System.Convert.ToString(value)[0]);
            int colourAngle = System.Convert.ToInt32(Math.Round((double)(char.ToUpper(firstCharacter) - 65) / 26 * 255, 0));

            // Get the R, G and B values of the performance standard
            int R1 = Math.Max(0, 220 - (int)Math.Ceiling(0.03 * colourAngle * colourAngle));
            int R2 = Math.Max(0, 220 - (int)Math.Ceiling(0.03 * (colourAngle - 225) * (colourAngle - 225)));
            int R = Math.Max(R1, R2);
            int G = Math.Max(0, 220 - (int)Math.Ceiling(0.03 * (colourAngle - 85) * (colourAngle - 85)));
            int B = Math.Max(0, 220 - (int)Math.Ceiling(0.03 * (colourAngle - 170) * (colourAngle - 170)));

            // Returns the appropriate solid colour brush
            return new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, (byte)B));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
