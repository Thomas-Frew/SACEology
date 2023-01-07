using SACEology.ViewModel;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class TeacherAddCoursePopUp : BasePopUp
    {
        public TeacherAddCoursePopUp()
        {
            InitializeComponent();
            DataContext = new TeacherAddCoursePopUpViewModel();
        }
    }
}
