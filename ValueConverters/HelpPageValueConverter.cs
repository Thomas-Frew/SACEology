using System;
using System.Globalization;
using SACEology.Pages;

namespace SACEology
{
    /// <summary>
    /// An integer to interface value converter
    /// </summary>
    public class HelpPageValueConverter : BaseValueConverter<HelpPageValueConverter>
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
            // Find the appropriate interface [REPLACE WITH NEW VARIABLE]
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.StudentPortal:
                    return new StudentPortalHelp();

                case ApplicationPage.StudentMyCourses:
                    return new StudentMyCoursesHelp();

                case ApplicationPage.StudentCourse:
                    return new StudentCourseHelp();

                case ApplicationPage.StudentAssignment:
                    return new StudentAssignmentHelp();

                case ApplicationPage.StudentMyDashboard:
                    return new StudentMyDashboardHelp();

                case ApplicationPage.StudentMyResults:
                    return new StudentMyResultsHelp();

                case ApplicationPage.TeacherPortal:
                    return new TeacherPortalHelp();

                case ApplicationPage.TeacherMyCourses:
                    return new TeacherMyCoursesHelp();

                case ApplicationPage.TeacherCourse:
                    return new TeacherCourseHelp();

                case ApplicationPage.TeacherAssignment:
                    return new TeacherAssignmentHelp();

                case ApplicationPage.TeacherMyCommunity:
                    return new TeacherMyCommunityHelp();

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
