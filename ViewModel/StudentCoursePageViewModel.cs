using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace SACEology.ViewModel
{
    class StudentCoursePageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The course's collection of assignments.
        /// </summary>
        public ObservableCollection<StudentAssignmentItemViewModel> Assignments { get; set; } = new ObservableCollection<StudentAssignmentItemViewModel>();

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

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentCoursePageViewModel()
        {
            // Load the course's properties and assignments
            LoadCourseProperties();
            DisplaySavedAssignments();

            // Initialise button relay commands
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
        }

        #endregion

        #region Private Helpers

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

        /// <summary>
        /// Displays all saved courses from the .csv database
        /// </summary>
        private void DisplaySavedAssignments()
        {
            // Clear the currently-displayed list of assignments
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
        }

        /// <summary>
        /// Displays a course on the view of current courses
        /// </summary>
        /// <param name="assignment">The assignment data</param>
        private void DisplayAssignment(List<string> assignment)
        {
            // Create a displayName for the assignment (featuring its assessment type and name)
            string displayName = assignment[(int)AProp.AssessmentType] + ": " + assignment[(int)AProp.Name];

            // Truncate the assignment's displayed name to 40 characters, followed by an ellipses (...)
            displayName = ValidationHelpers.TruncateString(displayName);

            // Create a new StudentAssignmentItemViewModel with the assignment's passed properties
            StudentAssignmentItemViewModel addedAssignment = new StudentAssignmentItemViewModel()
            {
                Course = Name,
                AssessmentType = assignment[(int)AProp.AssessmentType],
                Name = assignment[(int)AProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindAssignmentIcon(assignment),
                DueDate = assignment[(int)AProp.DueDate],
                Properties = FindAssignmentProperties(assignment)
            };
            
            // Add this assignment to the list of assignments
            Assignments.Add(addedAssignment); 
        }

        /// <summary>
        /// Finds an assignment's properties relative to the other assignments in its course.
        /// </summary>
        /// <param name="assignment">The assignment with discoverable properties</param>
        /// <returns>The assignment's collection of property badges</returns>
        public ObservableCollection<PropertyBadgeViewModel> FindAssignmentProperties(List<string> assignment)
        {
            // Create a new list of PropertyBadgeViewModels to store the current assignment's property badges
            ObservableCollection<PropertyBadgeViewModel> properties = new ObservableCollection<PropertyBadgeViewModel>();

            // If this assignment is the most recent, add a badge indicating this 
            if (PropertyHelpers.IsSoonestAssignment(assignment[(int)AProp.Course], assignment[(int)AProp.DueDate]))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf017", Description = "Most urgent assignment." });
            }

            // If this assignment is the most narrowly assessed, add a badge indicating this
            if (PropertyHelpers.IsNarrowestAssignment(assignment[(int)AProp.Course], assignment[(int)AProp.PerformanceStandards]))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf002", Description = "Most narrowly-assessed assignment." });
            }

            // If this badge is the most broadly assessed, add a badge indicate this
            if (PropertyHelpers.IsBroadestAssignment(assignment[(int)AProp.Course], assignment[(int)AProp.PerformanceStandards]))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf61f", Description = "Most broadly-assessed assignment." });
            }

            // If this assignemnt is the most heavily weighted, add a badge indicating this
            if (PropertyHelpers.IsMostWeightedAssignment(assignment[(int)AProp.Course], assignment[(int)AProp.Weight]))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf5cd", Description = "Most weighted assignment." });
            }

            // If this assignment has the shortest duration to complete, add a badge indicating this
            if (PropertyHelpers.IsShortestLengthAssignment(assignment[(int)AProp.Course], assignment[(int)AProp.StartingDate], assignment[(int)AProp.DueDate]))
            {
                properties.Add(new PropertyBadgeViewModel { Icon = "\uf252", Description = "Shortest-length assignment." });
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
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.StudentMyCourses);

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
