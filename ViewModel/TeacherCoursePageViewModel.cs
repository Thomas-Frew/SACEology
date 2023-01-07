using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SACEology.ViewModel
{
    class TeacherCoursePageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The course's collection of assignments.
        /// </summary>
        public ObservableCollection<TeacherAssignmentItemViewModel> Assignments { get; set; } = new ObservableCollection<TeacherAssignmentItemViewModel>();

        /// <summary>
        /// The course's subject code.
        /// </summary>
        public string SubjectCode { get; set; }

        /// <summary>
        /// The course's name.
        /// </summary>
        public string Name { get; set; } = Settings.Default.SelectedCourse;

        /// <summary>
        /// The course's icon.
        /// </summary>
        public string Icon { get; set; }

        public ObservableCollection<AspectBadgeViewModel> MissingAssessmentTypes { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

        public ObservableCollection<AspectBadgeViewModel> MissingPerformanceStandards { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

        public bool AssessmentTypesComplete { get; set; } = false;

        public bool PerformanceStandardsComplete { get; set; } = false;

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when an add course pop up is to be created.
        /// </summary>
        public ICommand AddAssignmentButtonCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public TeacherCoursePageViewModel()
        {
            // Load the course's properties and assignments
            LoadCourseProperties();
            DisplaySavedAssignments();
            UpdateMissingParameters();

            // Initialise button relay commands
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
            AddAssignmentButtonCommand = new RelayCommand(() => ShowAddAssignmentPopUp());

            // Hook into external events relevant to the course.
            DatabaseAggregator.OnAssignmentCreated += DisplaySavedAssignments;
            DatabaseAggregator.OnAssignmentDeleted += DisplaySavedAssignments;
        }

        /// <summary>
        /// Loads the course's properties from courseDatabase.csv.
        /// </summary>
        private void LoadCourseProperties()
        {
            // Load a database of all assignments
            List<List<string>> courseDatabase = DatabaseHelpers.LoadCourseDatabase();

            // Unpack this assignment's properties
            foreach (List<string> course in courseDatabase)
            {
                if (Name == course[(int)CProp.Name])
                {
                    SubjectCode = course[(int)CProp.SubjectCode];
                    Icon = DatabaseHelpers.FindCourseIcon(course);
                }
            }
        }

        #endregion

        #region Database Management

        /// <summary>
        /// Displays all saved courses from the .csv database
        /// </summary>
        private void DisplaySavedAssignments()
        {
            Assignments.Clear();

            // Load the master database of assignment
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // For each assignment in the database of assignments...
            foreach (List<string> assignment in assignmentDatabase)
            {
                // If the assignment belongs to the current course...
                if (Name == assignment[(int)AProp.Course])
                {
                    // Add the course to the view of courses
                    DisplayAssignment(assignment);
                }
            }

            UpdateMissingParameters();
        }

        /// <summary>
        /// Displays a course on the view of current courses
        /// </summary>
        /// <param name="course">The course data (subject code, name, icon)</param>
        /// <param name="date">The due date of the course's most recent assignment</param>
        /// <param name="msgs">The sum of new messages for this course</param>
        private void DisplayAssignment(List<string> assignment)
        {
            // Truncate the displayed name
            string displayName = assignment[(int)AProp.AssessmentType] + ": " + assignment[(int)AProp.Name];
            displayName = ValidationHelpers.TruncateString(displayName);

            // Load the message database
            List<List<string>> messageDatabase = DatabaseHelpers.LoadAssignmentMessageDatabase();

            // Initialise a variable for the number of messages for this assignment
            int messages = new int();

            // For each message in the message database
            foreach (List<string> message in messageDatabase)
            {
                // If the message is attributed to this assignment
                if (message[(int)AMProp.Assignment] == assignment[(int)AProp.Name])
                {
                    // Increment this assignment's message count
                    messages++;
                }
            }

            // Create a new assignment with all the necessary parameters
            TeacherAssignmentItemViewModel addedAssignment = new TeacherAssignmentItemViewModel()
            {
                Course = Name,
                AssessmentType = assignment[(int)AProp.AssessmentType],
                Name = assignment[(int)AProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindAssignmentIcon(assignment),
                NewMessages = messages.ToString(),
                DueDate = assignment[(int)AProp.DueDate]
            };

            // Add this assignment to the collection of assignments
            Assignments.Add(addedAssignment); 
        }

        /// <summary>
        /// Dynamically displays which parameters are missing.
        /// </summary>
        private void UpdateMissingParameters()
        {
            // Clear the missing course properties
            MissingAssessmentTypes.Clear();
            MissingPerformanceStandards.Clear();

            // Find the missing assessment types and performance standards for this assessment
            List<string> missingPerformanceStandards = ValidationHelpers.FindMissingPerformanceStandards(Name);
            List<string> missingAssessmentTypes = ValidationHelpers.FindMissingAssessmentTypes(Name);

            // If there are 0 missing performance standards and assessment types, then set PerformanceStandardsComplete or AssessmentTypesComplete to true
            AssessmentTypesComplete = missingAssessmentTypes.Count == 0 ? true : false;
            PerformanceStandardsComplete = missingPerformanceStandards.Count == 0 ? true : false;

            // For each performance standard, add to performance standard to the display...
            foreach (string standard in missingPerformanceStandards)
            {
                // Diplay the current, missing performance standard as an AspectBadge
                AspectBadgeViewModel badge = new AspectBadgeViewModel() { Code = standard, RootCode = standard.Remove(standard.Length - 1) };
                MissingPerformanceStandards.Add(badge);
            }

            // For each performance standard, add to performance standard to the display...
            foreach (string assessment in missingAssessmentTypes)
            {
                // Diplay the current, missing assessmentType as an AspectBadge
                AspectBadgeViewModel badge = new AspectBadgeViewModel() { Code = assessment, RootCode = assessment };
                MissingAssessmentTypes.Add(badge);
            }
        }

        #endregion

        #region Pop-Up Core

        /// <summary>
        /// Shows the "Add Course" pop up.
        /// </summary>
        private void ShowAddAssignmentPopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastAddAssessmentPopUpCreation();
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
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.TeacherMyCourses);

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
            // If a pop-up is not currently opened...
            if (!Settings.Default.PopUpOpen)
            {
                // Broadcast that a new help pop-up is to be created
                PopUpAggregator.BroadcastHelpPopUpCreation();
            }
        }

        #endregion
    }
}
