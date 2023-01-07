using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;

namespace SACEology.ViewModel
{
    class PortalPageViewModel : BaseViewModel
    {
        #region Public Commands

        /// <summary>
        /// The command for when any dashboard button is pressed, to navigate to the student portal.
        /// </summary>
        public ICommand NavigationButtonCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public PortalPageViewModel()
        {
            NavigationButtonCommand = new RelayParameterizedCommand((parameter) => HandleMenuRequest(parameter));
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
        }

        #endregion

        #region Navigation Core

        /// <summary>
        /// The command for when the back button is pressed, to naviagate the user backwards.
        /// </summary>
        public ICommand NavigateBackwardButtonCommand { get; set; }

        /// <summary>
        /// Navigates a page up, in this case, to the application's main menu.
        /// </summary>
        private void NavigateBackward()
        {
            // Switch to a the teacher course page
            Settings.Default.PreviousPage = Settings.Default.CurrentPage;
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.MainMenu);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }

        /// <summary>
        /// Sends the user to the main menu or previously opened page.
        /// </summary>
        private void HandleMenuRequest(object page)
        {
            // Write the current page as the previously opened page
            Settings.Default.PreviousPage = Settings.Default.CurrentPage;
            Settings.Default.CurrentPage = Convert.ToInt32(page);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }

        /// <summary>
        /// The command for when then "?" is pressed, to show the help pop-up.
        /// </summary>
        public ICommand HelpCommand { get; set; }

        /// <summary>
        /// Displays the help pop-up on the page.
        /// </summary>
        private void ShowHelpPopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastHelpPopUpCreation();
            }
        }

        #endregion
    }
}
