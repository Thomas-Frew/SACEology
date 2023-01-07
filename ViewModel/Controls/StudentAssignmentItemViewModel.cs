using SACEology.ViewModel.Base;
using System.Windows.Input;
using System;
using SACEology.Properties;
using System.Collections.ObjectModel;

namespace SACEology.ViewModel
{
    class StudentAssignmentItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The assignment's assessment type.
        /// </summary>
        public string AssessmentType { get; set; }

        /// <summary>
        /// The assignment's parent course.
        /// </summary>
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
        /// The number of new messages for this assignment.
        /// </summary>
        public string NewMessages { get; set; }

        /// <summary>
        /// The assignment's due date.
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// The assignment's property badges.
        /// </summary>
        public ObservableCollection<PropertyBadgeViewModel> Properties { get; set; } = new ObservableCollection<PropertyBadgeViewModel>();

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the eye button is pressed, to view the assignment in greater detail.
        /// </summary>
        public ICommand ViewAssignmentCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentAssignmentItemViewModel()
        {
            ViewAssignmentCommand = new RelayCommand(() => ViewAssignment());
        }

        #endregion

        #region Private Helpers

        private void ViewAssignment()
        {
            // Write the current page as the previously opened page
            Settings.Default.SelectedAssignment = Name;

            Settings.Default.PreviousPage = Settings.Default.CurrentPage;
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.StudentAssignment);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }

        #endregion
    }
}
