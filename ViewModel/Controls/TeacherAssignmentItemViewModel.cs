using SACEology.ViewModel.Base;
using System.Windows.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SACEology.Properties;

namespace SACEology.ViewModel
{
    class TeacherAssignmentItemViewModel : BaseViewModel
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

        #endregion

        #region Public Command

        /// <summary>
        /// The command for when the eye button is pressed, to view the assignment in greater detail.
        /// </summary>
        public ICommand ViewAssignmentCommand { get; set; }

        /// <summary>
        /// The command for when the bin button is pressed, to delete the assignment from the database.
        /// </summary>
        public ICommand DeleteAssignmentCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public TeacherAssignmentItemViewModel()
        {
            DeleteAssignmentCommand = new RelayCommand(() => DeleteAssignment());
            ViewAssignmentCommand = new RelayCommand(() => ViewAssignment());
        }

        #endregion

        #region Private Helpers

        private void ViewAssignment()
        {
            // Write the current page as the previously opened page
            Settings.Default.SelectedAssignment = Name;

            Settings.Default.PreviousPage = Settings.Default.CurrentPage;
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.TeacherAssignment);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }

        private void DeleteAssignment()
        {
            // Load the master database of assignment
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            int removedAssignmentIndex = new int();

            // Find all occurences of the course across the course and assignment databases
            foreach (int i in Enumerable.Range(0, assignmentDatabase.Count))
            {
                if (assignmentDatabase[i][(int)AProp.Course] == Course && assignmentDatabase[i][(int)AProp.Name] == Name)
                {
                    removedAssignmentIndex = i;
                }  
            }

            // Remove the course and all assignments under the course's name
            assignmentDatabase.RemoveAt(removedAssignmentIndex);

            DatabaseHelpers.SaveAssignmentDatabase(assignmentDatabase);
            DatabaseAggregator.BroadcastAssignmentDeletion();
        }

        #endregion
    }
}
