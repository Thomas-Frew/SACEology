using SACEology.ViewModel.Base;
using System.Windows.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SACEology.Properties;
using System.Collections.ObjectModel;

namespace SACEology.ViewModel
{
    class StudentCourseItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The course's subject code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The course's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The course's name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The course's FontAwesome icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The number of new messages for this course.
        /// </summary>
        public string NewMessages { get; set; }

        /// <summary>
        /// The due date of the course's next assesment.
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// The course's property badges.
        /// </summary>
        public ObservableCollection<PropertyBadgeViewModel> Properties { get; set; } = new ObservableCollection<PropertyBadgeViewModel>();

        #endregion

        #region Public Command

        public ICommand ViewCourseCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentCourseItemViewModel()
        {
            ViewCourseCommand = new RelayCommand(() => ViewCourse());

        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Opens a TeacherCoursePage when this control is clicked, to view its information in greater detail.
        /// </summary>
        private void ViewCourse()
        {
            // Pass the course name and properties through the application's settings
            Settings.Default.SelectedCourse = Name;

            // Find the selected course's subject and pass it through the application's settings
            List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();
            foreach (List<string> subject in subjectDatabase)
            {
                if (Code == subject[(int)SProp.SubjectCode])
                {
                    Settings.Default.SelectedSubject = subject[(int)SProp.Name];
                }
            }
           
            // Set the previous page to the current page
            Settings.Default.PreviousPage = Settings.Default.CurrentPage;

            // Set the current page to a new, student course page
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.StudentCourse);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }


        #endregion
    }
}
