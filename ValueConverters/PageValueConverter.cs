using System;
using System.Globalization;
using SACEology.Pages;

namespace SACEology
{
    /// <summary>
    /// An integer to interface value converter
    /// </summary>
    public class PageValueConverter : BaseValueConverter<PageValueConverter>
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
            // Find the appropriate interface
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.MainMenu:
                    return new MainMenuPage();

                case ApplicationPage.StudentPortal:
                    return new StudentPortalPage();

                case ApplicationPage.StudentMyCourses:
                    return new StudentMyCoursesPage();

                case ApplicationPage.StudentCourse:
                    return new StudentCoursePage();

                case ApplicationPage.StudentAssignment:
                    return new StudentAssignmentPage();

                case ApplicationPage.StudentMyDashboard:
                    return new StudentMyDashboardPage();

                case ApplicationPage.StudentMyResults:
                    return new StudentMyResultsPage();

                case ApplicationPage.TeacherPortal:
                    return new TeacherPortalPage();

                case ApplicationPage.TeacherMyCourses:
                    return new TeacherMyCoursesPage();

                case ApplicationPage.TeacherCourse:
                    return new TeacherCoursePage();

                case ApplicationPage.TeacherAssignment:
                    return new TeacherAssignmentPage();

                case ApplicationPage.TeacherMyCommunity:
                    return new TeacherMyCommunityPage();

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
