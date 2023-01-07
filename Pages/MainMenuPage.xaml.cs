using SACEology.Properties;
using SACEology.ViewModel;
using System.Windows.Controls;
using System.Windows;

namespace SACEology.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : BasePage
    {
        public MainMenuPage()
        {
            InitializeComponent();
            DataContext = new MainMenuPageViewModel();

            PopUpAggregator.OnCreditsPopUpCreation += ShowCreditsPopUp;
            PopUpAggregator.OnSettingsPopUpCreation += ShowSettingsPopUp;

            Settings.Default.CurrentPage = Settings.Default.PreviousPage = 0;
            Settings.Default.Save();
        }

        /// <summary>
        /// Displays a new "Credits" pop-up on the page.
        /// </summary>
        private void ShowCreditsPopUp()
        {
            CreditsPopUp popUp = new CreditsPopUp();
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 10);
            Grid.SetColumnSpan(popUp, 10);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }

        /// <summary>
        /// Displays a new "Settings" pop-up on the page.
        /// </summary>
        private void ShowSettingsPopUp()
        {
            SettingsPopUp popUp = new SettingsPopUp();
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 10);
            Grid.SetColumnSpan(popUp, 10);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
