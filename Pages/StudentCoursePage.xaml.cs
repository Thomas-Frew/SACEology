using SACEology.ViewModel;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentCoursePage : BasePage
    {
        public StudentCoursePage()
        {
            InitializeComponent();
            DataContext = new StudentCoursePageViewModel();
        }
    }
}
