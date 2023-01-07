using SACEology.ViewModel.Base;
using System.Collections.Generic;

namespace SACEology.ViewModel
{
    class AspectBadgeViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The badge's text.
        /// </summary>
        public string Code { get; set; }

        public string RootCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = "Test";

        /// <summary>
        /// Whether the badge is selected in a listbox or listview.
        /// </summary>
        public bool IsSelected { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public AspectBadgeViewModel()
        {

        }

        #endregion
    }
}
