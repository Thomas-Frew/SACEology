using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;

namespace SACEology.ViewModel
{
    class WindowViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A private variable storing the window's height.
        /// </summary>
        public int _windowHeight { get; set; } = 500;

        /// <summary>
        /// The window's current height.
        /// </summary>
        public int WindowHeight
        {
            get
            {
                return _windowHeight;
            }
            set
            {
                _windowHeight = value;

                Settings.Default.WindowHeight = value;
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// The window's minimum width.
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 850;

        /// <summary>
        /// The window's minimum height.
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 600;

        /// <summary>
        /// The currently selected page.
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = 0;

        #endregion

        #region Public Commands

        public ICommand HelpCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public WindowViewModel()
        {
            // If currentPage setting changes, and the page is animated out...
            BasePage.PageAnimatedOut += (sender, e) =>
            {
                // Set CurrentPage to the page saved in the currentPage setting
                ChangePage(Settings.Default.CurrentPage);
            };
        }

        #endregion

        #region Commands and Helpers

        /// <summary>
        /// Changes the current page variable.
        /// </summary>
        /// <param name="cInterface">The index of the current page</param>
        private void ChangePage(int cInterface)
        {
            CurrentPage = (ApplicationPage)(cInterface);
        }

        #endregion
    }
}
