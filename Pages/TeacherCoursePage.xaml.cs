using SACEology.ViewModel;
using System.Windows.Controls;
using System.Windows;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for TeacherCoursePage.xaml
    /// </summary>
    public partial class TeacherCoursePage : BasePage
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        public TeacherCoursePage()
        {
            // Open the page
            InitializeComponent();

            // Connect the page to a new TeacherCourseViewModel
            DataContext = new TeacherCoursePageViewModel();

            PopUpAggregator.OnAddAssessmentPopUpCreation += ShowAddAssessmentPopUp;
        }

        private void ShowAddAssessmentPopUp()
        {
            TeacherAddAssignmentPopUp popUp = new TeacherAddAssignmentPopUp();
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 3);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
