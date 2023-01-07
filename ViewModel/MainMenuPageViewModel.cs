using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;

namespace SACEology.ViewModel
{
    class MainMenuPageViewModel : BaseViewModel
    {
        #region Public Commands

        /// <summary>
        /// The command for when the "Student Portal" button is pressed, to navigate to the student portal.
        /// </summary>
        public ICommand StudentPortalButtonCommand { get; set; }

        /// <summary>
        /// The command for when the "Teacher Portal" button is pressed, to navigate to the student portal.
        /// </summary>
        public ICommand TeacherPortalButtonCommand { get; set; }

        /// <summary>
        /// The command for when the "Credits" button is pressed, to navigate to the student portal.
        /// </summary>
        public ICommand CreditsButtonCommand { get; set; }

        /// <summary>
        /// The command for when the "Settings" button is pressed, to navigate to the student portal.
        /// </summary>
        public ICommand SettingsButtonCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public MainMenuPageViewModel()
        {
            StudentPortalButtonCommand = new RelayParameterizedCommand((parameter) => HandleMenuRequest(parameter));
            TeacherPortalButtonCommand = new RelayParameterizedCommand((parameter) => HandleMenuRequest(parameter));
            CreditsButtonCommand = new RelayCommand(() => ShowCreditsPopUp());
            SettingsButtonCommand = new RelayCommand(() => ShowSettingsPopUp());
        }

        #endregion

        #region Commands and Helpers

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
        /// Broadcasts that a "Credits" pop-up should be created
        /// </summary>
        private void ShowCreditsPopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastCreditsPopUpCreation();
            }
        }

        /// <summary>
        /// Broadcasts that a "Settings" pop-up should be created
        /// </summary>
        private void ShowSettingsPopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastSettingsPopUpCreation();
            }
        }

        #endregion
    }
}
