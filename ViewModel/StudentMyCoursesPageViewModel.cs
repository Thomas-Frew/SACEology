using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SACEology.ViewModel
{
    class StudentMyCoursesPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The observable collection of courses to the view
        /// </summary>
        public ObservableCollection<StudentCourseItemViewModel> Courses { get; set; } = new ObservableCollection<StudentCourseItemViewModel>();

        #endregion

        #region Public Commands

        public ICommand ViewCourseButtonCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentMyCoursesPageViewModel()
        {
            // Initialise button relay commands
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());

            DisplaySavedCourses();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Displays all saved courses from the .csv database
        /// </summary>
        private void DisplaySavedCourses()
        {
            Courses.Clear();

            // Load the master database of courses
            List<List<string>> courseDatabase = DatabaseHelpers.LoadCourseDatabase();

            // Load the master database of assignment
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Load the master database of assignment
            List<List<string>> messageDatabase = DatabaseHelpers.LoadAssignmentMessageDatabase();

            // For each course in the database of courses...
            foreach (List<string> course in courseDatabase)
            {
                // Initialise calculated properties
                int messageSum = 0;
                string mostRecentDate = "-";

                // For each assignment in the database of assignments...
                foreach (List<string> assignment in assignmentDatabase)
                {
                    // If the assignment belongs to the current course...
                    if (assignment[(int)AProp.Course] == course[(int)CProp.Name])
                    {
                        // Calculate the course's properties
                        mostRecentDate = assignment[(int)AProp.DueDate];
                    }
                }

                if (messageDatabase.Count > 0)
                {
                    foreach (List<string> message in messageDatabase)
                    {
                        if (message[(int)AMProp.Course] == course[(int)CProp.Name])
                        {
                            messageSum++;
                        }
                    }
                }

                // Add the course to the view of courses
                DisplayCourse(course, mostRecentDate, messageSum);
            }    
        }

        /// <summary>
        /// Displays a course on the view of current courses
        /// </summary>
        /// <param name="course">The course data (subject code, name, icon)</param>
        /// <param name="date">The due date of the course's most recent assignment</param>
        /// <param name="msgs">The sum of new messages for this course</param>
        public void DisplayCourse(List<string> course, string date, int msgs)
        {
            // Truncate the displayed name
            string displayName = course[(int)CProp.SubjectCode] + ": " + course[(int)CProp.Name];
            displayName = ValidationHelpers.TruncateString(displayName);

            StudentCourseItemViewModel addedCourse = new StudentCourseItemViewModel()
            {
                Code = course[(int)CProp.SubjectCode],
                Name = course[(int)CProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindCourseIcon(course),
                NewMessages = msgs.ToString(),
                DueDate = date,
                Properties = FindCourseProperties(course[(int)CProp.Name])
            };

            Courses.Add(addedCourse);
        }

        /// <summary>
        /// Finds all property badges applicable to the current course and returns all their view models as an observable collection.
        /// </summary>
        /// <param name="name">The name of the currently selected course</param>
        /// <returns>A </returns>
        public ObservableCollection<PropertyBadgeViewModel> FindCourseProperties(string name)
        {
            ObservableCollection<PropertyBadgeViewModel> properties = new ObservableCollection<PropertyBadgeViewModel>();

            if (PropertyHelpers.HasSoonestAssignment(name))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf017", Description = "Most urgent assignment." });
            }
            if (PropertyHelpers.IsMostPopulatedCourse(name))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf468", Description = "Most assignments." });
            }

            return properties;
        }

        #endregion

        #region Navigation Core

        /// <summary>
        /// The command for when the back button is pressed, to naviagate the user backwards.
        /// </summary>
        public ICommand NavigateBackwardButtonCommand { get; set; }

        private void NavigateBackward()
        {
            // Switch to a the teacher course page
            Settings.Default.PreviousPage = Settings.Default.CurrentPage;
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.StudentPortal);

            // Save the new page states, activating the animations
            Settings.Default.Save();
        }

        /// <summary>
        /// The command for when then "?" is pressed, to show the help pop-up.
        /// </summary>
        public ICommand HelpCommand { get; set; }

        /// <summary>
        /// Displays the help pop-up on the page.
        /// </summary>
        private void ShowHelpPopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastHelpPopUpCreation();
            }
        }

        #endregion
    }
}
