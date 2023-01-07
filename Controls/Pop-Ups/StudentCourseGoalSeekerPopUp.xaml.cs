namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentCourseGoalSeekerPopUp : BasePopUp
    {
        public StudentCourseGoalSeekerPopUp()
        {
            InitializeComponent();
            DataContext = new StudentCourseGoalSeekerPopUpViewModel();
        }
    }
}
