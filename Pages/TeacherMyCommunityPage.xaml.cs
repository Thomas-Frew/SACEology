using SACEology.ViewModel;
using System.Windows.Controls;
using System.Windows;
using SACEology.Properties;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class TeacherMyCommunityPage : BasePage
    {
        public TeacherMyCommunityPage()
        {
            InitializeComponent();
            DataContext = new TeacherMyCommunityPageViewModel();

            // Set the selected message server as community
            Settings.Default.SelectedMessageServer = (int)MessageServer.Community;
            Settings.Default.Save();

            // Scroll the messages scroll viewer to the bottom (possible with an attached property but easier in code behind)
            MessagesScrollViewer.ScrollToBottom();

            PopUpAggregator.OnSendMessagePopUpCreation += ShowSendMessagePopUp;
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
