using SACEology.Properties;
using SACEology.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initalise the window and its view model
            InitializeComponent();
            DataContext = new WindowViewModel();

            // Listen out for error, confirmation and felp pop up creation
            PopUpAggregator.OnErrorPopUpCreation += ShowErrorPopUp;
            PopUpAggregator.OnConfirmationPopUpCreation += ShowConfirmationPopUp;
            PopUpAggregator.OnHelpPopUpCreation += ShowHelpPopUp;

            // Begin the application with pop-ups in a closed state
            Settings.Default.PopUpOpen = false;
            Settings.Default.Save();
        }

        #region Private Helpers

        /// <summary>
        /// Displays an error pop up, when an error has occured.
        /// </summary>
        /// <param name="message"></param>
        private void ShowErrorPopUp(string message)
        {
            ErrorPopUp popUp = new ErrorPopUp(message);
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 2);
            popUp.VerticalAlignment = VerticalAlignment.Bottom;
        }

        /// <summary>
        /// Confirms that the user has completed an action.
        /// </summary>
        /// <param name="message"></param>
        private void ShowConfirmationPopUp(string message)
        {
            ConfirmationPopUp popUp = new ConfirmationPopUp(message);
            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 2);
            popUp.VerticalAlignment = VerticalAlignment.Bottom;
        }

        /// <summary>
        /// Displays a help pop-up
        /// </summary>
        private void ShowHelpPopUp()
        {
            HelpPopUp popUp = new HelpPopUp();

            mainframe.Children.Add(popUp);
            Grid.SetRowSpan(popUp, 2);
            popUp.VerticalAlignment = VerticalAlignment.Stretch;
            popUp.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        /// <summary>
        /// Removes the history of a frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFrameHistory(object sender, NavigationEventArgs e)
        {
            Frame frame = sender as Frame;
            frame.NavigationService.RemoveBackEntry();
        }

        #endregion
    }
}
