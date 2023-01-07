using SACEology.Properties;
using SACEology.ViewModel;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class TeacherPortalPage : BasePage
    {
        public TeacherPortalPage()
        {
            InitializeComponent();
            DataContext = new PortalPageViewModel();

            // Save that a teacher is using the app
            Settings.Default.UserStatus = (int)UserStatus.Teacher;
        }
    }
}
