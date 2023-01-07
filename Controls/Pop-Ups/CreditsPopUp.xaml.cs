namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class CreditsPopUp : BasePopUp
    {
        public CreditsPopUp()
        {
            InitializeComponent();

            DataContext = new CreditsPopUpViewModel();
        }
    }
}
