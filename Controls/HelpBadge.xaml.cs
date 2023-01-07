using System.Windows.Controls;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for TeacherCourseItem.xaml
    /// </summary>
    public partial class HelpBadge : UserControl
    {
        public HelpBadge()
        {
            InitializeComponent();

            DataContext = new HelpPopUpViewModel();
        }
    }
}
