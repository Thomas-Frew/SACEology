namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class SendMessagePopUp : BasePopUp
    {
        public SendMessagePopUp()
        {
            InitializeComponent();
            DataContext = new SendMessagePopUpViewModel();
        }
    }
}
