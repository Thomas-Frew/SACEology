using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SACEology.ViewModel
{
    class TeacherMyCoursesPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The observable collection of courses to the view
        /// </summary>
        public ObservableCollection<TeacherCourseItemViewModel> Courses { get; set; } = new ObservableCollection<TeacherCourseItemViewModel>();

        /// <summary>
        /// The number of courses owned by the teacher
        /// </summary>
        public int CourseCount { get; set; }

        /// <summary>
        /// The number of non-SACE-approved (incomplete) courses owned by the teacher
        /// </summary>
        public int IncompleteCourseCount { get; set; }

        /// <summary>
        /// The total number of messages acorss all the teacher's courses
        /// </summary>
        public int NewMessageCount { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when an add course pop up is to be created.
        /// </summary>
        public ICommand AddCourseButtonCommand { get; set; }

        public ICommand ViewCourseButtonCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public TeacherMyCoursesPageViewModel()
        {
            // Initialise button relay commands
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
            AddCourseButtonCommand = new RelayCommand(() => ShowAddCoursePopUp());

            DisplaySavedCourses();
            UpdateMyCoursesProperties();

            DatabaseAggregator.OnCourseCreated += DisplaySavedCourses;
            DatabaseAggregator.OnCourseDeleted += DisplaySavedCourses;
        }

        private void UpdateMyCoursesProperties()
        {
            // Load the master database of courses
            List<List<string>> courseDatabase = DatabaseHelpers.LoadCourseDatabase();

            // Load the master database of assignment
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Set the course count to the number of courses in the database
            CourseCount = courseDatabase.Count();

            // Reset the incomplete course and new message counts
            IncompleteCourseCount = 0;
            NewMessageCount = 0;

            // For every course in the course database...
            foreach (List<string> course in courseDatabase)
            {
                List<string> missingAssessmentTypes = ValidationHelpers.FindMissingAssessmentTypes(course[(int)CProp.Name]);
                List<string> missingPerformanceStandards = ValidationHelpers.FindMissingPerformanceStandards(course[(int)CProp.Name]);

                if (missingAssessmentTypes.Count > 0 || missingPerformanceStandards.Count > 0)
                {
                    IncompleteCourseCount++;
                }
            }

            NewMessageCount = DatabaseHelpers.LoadAssignmentMessageDatabase().Count;
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

            // Load the master database of assignments
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Load the master database of messages
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
                        mostRecentDate = new string[] { mostRecentDate, assignment[(int)AProp.DueDate] }.Max();
                    }
                }

                // For each message in the database of messages...
                foreach (List<string> message in messageDatabase)
                {
                    // If the assignment belongs to the current course...
                    if (message[(int)AMProp.Course] == course[(int)CProp.Name])
                    {
                        messageSum++;
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
        private void DisplayCourse(List<string> course, string date, int msgs)
        {
            // Truncate the displayed name
            string displayName = course[(int)CProp.SubjectCode] + ": " + course[(int)CProp.Name];
            displayName = ValidationHelpers.TruncateString(displayName);

            TeacherCourseItemViewModel addedCourse = new TeacherCourseItemViewModel()
            {
                Code = course[(int)CProp.SubjectCode],
                Name = course[(int)CProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindCourseIcon(course),
                NewMessages = msgs.ToString(),
                DueDate = date
            };

            Courses.Add(addedCourse);
            UpdateMyCoursesProperties();
        }

        #endregion

        #region Pop-Up Core

        /// <summary>
        /// Shows the "Add Course" pop up.
        /// </summary>
        private void ShowAddCoursePopUp()
        {
            // If a pop-up is not currently opened...
            if (!Settings.Default.PopUpOpen)
            {
                // Broadcast that a new "Add Course" pop-up is to be created
                PopUpAggregator.BroadcastAddCoursePopUpCreation();
            }
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
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.MainMenu);

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
