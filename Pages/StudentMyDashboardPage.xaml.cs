using SACEology.ViewModel;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentMyDashboardPage : BasePage
    {
        public StudentMyDashboardPage()
        {
            InitializeComponent();
            DataContext = new StudentMyDashboardPageViewModel();
        }
    }
}
