using System.Collections.Generic;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentGradeBreakdownPopUp : BasePopUp
    {
        public StudentGradeBreakdownPopUp(List<string> standards, List<double> occurences, List<double> grades)
        {
            InitializeComponent();
            DataContext = new StudentGradeBreakdownPopUpViewModel(standards, occurences, grades);
        }
    }
}
