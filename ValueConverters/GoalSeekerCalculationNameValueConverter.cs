using System;
using System.Globalization;
using SACEology.Pages;

namespace SACEology
{
    /// <summary>
    /// An integer to interface value converter
    /// </summary>
    public class GoalSeekerCalculationNameValueConverter : BaseValueConverter<GoalSeekerCalculationNameValueConverter>
    {
        /// <summary>
        /// Converts from integers to interfaces
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the appropriate title for the goal seeker calculation
            switch ((GoalSeekerCalculation)value)
            {
                case GoalSeekerCalculation.MostEfficient:
                    return "Most Efficient";

                case GoalSeekerCalculation.GuaranteedEntry:
                    return "Guaranteed Entry";

                case GoalSeekerCalculation.FastestActionable:
                    return "Fastest-Actionable";

                case GoalSeekerCalculation.SlowestActionable:
                    return "Slowest-Actionable";

                case GoalSeekerCalculation.AntiExam:
                    return "Anti-Exam";

                case GoalSeekerCalculation.AntiFolio:
                    return "Anti-Folio";


                // If no interface exists under the selected integer, return nothing
                default:
                    return null;
            }
        }

        /// <summary>
        /// Coverts from interfaces back to integers
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
