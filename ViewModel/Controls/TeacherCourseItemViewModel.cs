using SACEology.ViewModel.Base;
using System.Windows.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SACEology.Properties;

namespace SACEology.ViewModel
{
    class TeacherCourseItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The course's subject code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The course's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The course's name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The course's FontAwesome icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The number of new messages for this course
        /// </summary>
        public string NewMessages { get; set; }

        /// <summary>
        /// The due date of the course's next assesment
        /// </summary>
        public string DueDate { get; set; }

        #endregion

        #region Public Command

        public ICommand ViewCourseCommand { get; set; }

        public ICommand DeleteCourseCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public TeacherCourseItemViewModel()
        {
            DeleteCourseCommand = new RelayCommand(() => DeleteCourse());
            ViewCourseCommand = new RelayCommand(() => ViewCourse());
        }

        #endregion

        #region Private Helpers

        private void ViewCourse()
        {
            // Pass the course name and properties through the application's settings
            Settings.Default.SelectedCourse = Name;

            List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();
            foreach (List<string> subject in subjectDatabase)
            {
                if (Code == subject[(int)SProp.SubjectCode])
                {
                    Settings.Default.SelectedSubject = subject[(int)SProp.Name];
                }

            }
           
            Settings.Default.PreviousPage = Settings.Default.CurrentPage;
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.TeacherCourse);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }

        private void DeleteCourse()
        {
            // Load the master database of courses
            List<List<string>> courseDatabase = DatabaseHelpers.LoadCourseDatabase();

            // Load the master database of assignment
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            int removedCourseIndex = new int();
            List<int> removedAssignmentIndices = new List<int>();

            // Find all occurences of the course across the course and assignment databases
            foreach (int i in Enumerable.Range(0, courseDatabase.Count))
            {
                if (courseDatabase[i][(int)CProp.Name] == Name)
                {
                    removedCourseIndex = i;
                }  
            }

            foreach (int i in Enumerable.Range(0, assignmentDatabase.Count))
            {
                if (assignmentDatabase[i][(int)CProp.Name] == Name)
                {
                    removedAssignmentIndices.Add(i);
                }
            }
            removedAssignmentIndices = removedAssignmentIndices.OrderByDescending(i => i).ToList();

            // Remove the course and all assignments under the course's name
            courseDatabase.RemoveAt(removedCourseIndex);

            foreach (int index in removedAssignmentIndices)
            {
                assignmentDatabase.RemoveAt(index);
            }

            DatabaseHelpers.SaveCourseDatabase(courseDatabase);
            DatabaseHelpers.SaveAssignmentDatabase(assignmentDatabase);
            DatabaseAggregator.BroadcastCourseDeletion();
        }

        #endregion
    }
}
