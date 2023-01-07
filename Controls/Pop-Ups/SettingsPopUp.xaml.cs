namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class SettingsPopUp : BasePopUp
    {
        public SettingsPopUp()
        {
            InitializeComponent();

            DataContext = new SettingsPopViewModel();
        }
    }
}
