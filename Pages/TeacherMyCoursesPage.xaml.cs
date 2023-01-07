using SACEology.ViewModel;
using System.Windows.Controls;
using System.Windows;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class TeacherMyCoursesPage : BasePage
    {
        public TeacherMyCoursesPage()
        {
            // Open the page
            InitializeComponent();

            // Connect the page to a new instance of TeacherMyCoursesPageViewModel
            DataContext = new TeacherMyCoursesPageViewModel();

            // When an "Add Course" pop-up is called to be displayed, run the command to display it.
            PopUpAggregator.OnAddCoursePopUpCreation += ShowAddCoursePopUp;
        }

        /// <summary>
        /// Displays a new "Add Course" pop-up on the page.
        /// </summary>
        private void ShowAddCoursePopUp()
        {
            // Create a new instance of an "Add Course" pop-up
            TeacherAddCoursePopUp popUp = new TeacherAddCoursePopUp();

            // Add the pop-up to the main grid for the page
            mainframe.Children.Add(popUp);

            // Make the pop-up span the entire grid (3 rows)
            Grid.SetRowSpan(popUp, 3);
        }
    }
}
