using SACEology.Properties;
using SACEology.ViewModel;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class MessagesPopUp : BasePopUp
    {
        public MessagesPopUp()
        {
            InitializeComponent();
            DataContext = new MessagesPopUpViewModel();

            // Set the selected message server as community
            Settings.Default.SelectedMessageServer = (int)MessageServer.Assignment;

            // Scroll the messages scroll viewer to the bottom (possible with an attached property but easier in code behind)
            MessagesScrollViewer.ScrollToBottom();
        }
    }
}
