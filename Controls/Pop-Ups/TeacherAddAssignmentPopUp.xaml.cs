using SACEology.ViewModel;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class TeacherAddAssignmentPopUp : BasePopUp
    {
        public TeacherAddAssignmentPopUp()
        {
            InitializeComponent();
            DataContext = new TeacherAddAssignmentPopUpViewModel();
        }
    }
}
