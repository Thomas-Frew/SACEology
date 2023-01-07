using System;
using System.Collections.Generic;
using System.Globalization;

namespace SACEology
{
    /// <summary>
    /// A converter that takes in an RGB string such as FF00FF and converts it to a WPF brush
    /// </summary>
    public class RootCodeToTitleConverter : BaseValueConverter<RootCodeToTitleConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Initialise the passed code as a string variable. 
            string code = value.ToString();

            // Load the performance standard database
            List<List<string>> PerformanceStandardDatabase = DatabaseHelpers.LoadperformanceStandardDatabase();

            // If this badge's code exists in the performance standard database, return its name
            foreach (List<string> standard in PerformanceStandardDatabase)
            {
                if (code == standard[(int)PSProp.Code])
                {
                    return standard[(int)PSProp.Name];
                }
            }

            // Load the assessment type database
            List<List<string>> AssessmentTypeDatabase = DatabaseHelpers.LoadAssessmentTypeDatabase();

            // If this badge's code exists in the assessment type database, return its name
            foreach (List<string> type in AssessmentTypeDatabase)
            {
                if (code == type[(int)ATProp.Code])
                {
                    return type[(int)ATProp.Name];
                }
            }

            // If this badge's description is still not found, return it as such
            return "Not Found";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
