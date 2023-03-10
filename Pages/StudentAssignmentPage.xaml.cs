using SACEology.ViewModel;
using System.Windows.Controls;
using System.Windows;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class StudentAssignmentPage : BasePage
    {
        public StudentAssignmentPage()
        {
            InitializeComponent();
            DataContext = new StudentAssignmentPageViewModel();

            PopUpAggregator.OnMessagesPopUpCreation += ShowMessagesPopUp;
            PopUpAggregator.OnSendMessagePopUpCreation += ShowSendMessagePopUp;
        }

        private void ShowMessagesPopUp()
        {
            MessagesPopUp popUp = new MessagesPopUp();
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 10);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void ShowSendMessagePopUp()
        {
            SendMessagePopUp popUp = new SendMessagePopUp();
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 10);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
