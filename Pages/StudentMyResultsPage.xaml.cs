using SACEology.ViewModel;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentMyResultsPage : BasePage
    {
        public StudentMyResultsPage()
        {
            InitializeComponent();
            DataContext = new StudentMyResultsPageViewModel();

            PopUpAggregator.OnGradeBreakdownPopUpCreation += ShowGradeBreakdownPopUp;
            PopUpAggregator.OnCourseGoalSeekerPopUpCreation += ShowCourseGoalSeekerPopUp;
        }

        private void ShowGradeBreakdownPopUp(List<string> standards, List<double> occurences, List<double> grades)
        {
            StudentGradeBreakdownPopUp popUp = new StudentGradeBreakdownPopUp(standards, occurences, grades);
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 5);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void ShowCourseGoalSeekerPopUp()
        {
            StudentCourseGoalSeekerPopUp popUp = new StudentCourseGoalSeekerPopUp();
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 5);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
