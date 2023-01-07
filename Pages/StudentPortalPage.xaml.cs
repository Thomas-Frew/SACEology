using SACEology.Properties;
using SACEology.ViewModel;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentPortalPage : BasePage
    {
        public StudentPortalPage()
        {
            InitializeComponent();
            DataContext = new PortalPageViewModel();

            // Save that a student is using the app
            Settings.Default.UserStatus = (int)UserStatus.Student;
        }
    }
}
