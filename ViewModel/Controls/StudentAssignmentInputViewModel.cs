using SACEology.ViewModel.Base;
using static SACEology.ValidationHelpers;

namespace SACEology.ViewModel
{
    class StudentAssignmentInputViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The assignment's assessment type.
        /// </summary>
        public string AssessmentType { get; set; }

        public string Course { get; set; }

        /// <summary>
        /// The assignment's FontAwesome icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The assignment's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The assignment's display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The assignment's weight.
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Whether this assignment grade input is readonly or able to be write.
        /// </summary>
        public bool IsReadOnly { get; set; } = true;

        /// <summary>
        /// A private variable storing the grade achieved for the assignment.
        /// </summary>
        private string _grade { get; set; }

        /// <summary>
        /// The grade acheived for the assignment.
        /// </summary>
        public string Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                _grade = ValidateGrade(value);

                // If the control has been edited by the user (not initialised from code, where it is read only)...
                if (!IsReadOnly)
                {
                    // Broadcast that a grade has been edited
                    ValidationAggregator.BroadcastGradeEditing();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentAssignmentInputViewModel() { }

        #endregion
    }
}
