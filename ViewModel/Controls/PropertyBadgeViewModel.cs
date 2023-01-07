using SACEology.ViewModel.Base;

namespace SACEology.ViewModel
{
    class PropertyBadgeViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The badge's icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The description of this badge property.
        /// </summary>
        public string Description { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public PropertyBadgeViewModel() { }

        #endregion
    }
}
