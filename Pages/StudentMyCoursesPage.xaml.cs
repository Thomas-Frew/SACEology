using SACEology.ViewModel;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentMyCoursesPage : BasePage
    {
        public StudentMyCoursesPage()
        {
            InitializeComponent();
            DataContext = new StudentMyCoursesPageViewModel();
        }

    }
}
