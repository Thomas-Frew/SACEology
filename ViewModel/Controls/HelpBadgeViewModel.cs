using SACEology.ViewModel.Base;
using System.Collections.Generic;

namespace SACEology.ViewModel
{
    class HelpBadgeViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The badge's description.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public HelpBadgeViewModel() { }

        #endregion
    }
}
