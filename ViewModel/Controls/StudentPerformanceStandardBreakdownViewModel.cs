using SACEology.ViewModel.Base;

namespace SACEology.ViewModel
{
    class StudentPerformanceStandardBreakdownViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The assignment's badge
        /// </summary>
        public AspectBadgeViewModel PerformanceStandardBadge { get; set; }

        /// <summary>
        /// The assignment's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of times this performance standard occurs in the course
        /// </summary>
        public string Occurences { get; set; }

        /// <summary>
        /// The grade acheived for this performance standard
        /// </summary>
        public string Grade { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentPerformanceStandardBreakdownViewModel() { }

        #endregion
    }
}
