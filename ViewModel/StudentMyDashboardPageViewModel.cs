using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace SACEology.ViewModel
{
    class StudentMyDashboardPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The observable collection of the most recent assignments accessible to the view.
        /// </summary>
        public ObservableCollection<StudentAssignmentItemViewModel> MostRecentAssignments { get; set; } = new ObservableCollection<StudentAssignmentItemViewModel>();

        /// <summary>
        /// The overall percentage of coursework completed by the student.
        /// </summary>
        public string CourseworkCompletion { get; set; }

        /// <summary>
        /// The percentage of formative tasks completed by the student.
        /// </summary>
        public string FormativeCourseworkCompletion { get; set; }

        /// <summary>
        /// The percentage of minor tasks completed by the student.
        /// </summary>
        public string MinorCourseworkCompletion { get; set; }

        /// <summary>
        /// The percentage of moderate tasks completed by the student.
        /// </summary>
        public string ModerateCourseworkCompletion { get; set; }

        /// <summary>
        /// The percentage of major tasks completed by the student.
        /// </summary>
        public string MajorCourseworkCompletion { get; set; }

        public ObservableCollection<AspectBadgeViewModel> ImportantPerformanceStandards { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

        public ObservableCollection<AspectBadgeViewModel> ImportantAssessmentTypes { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

        #endregion

        #region Public Commands

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentMyDashboardPageViewModel()
        {
            // Initialise button relay commands
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());

            DisplayCourseworkCompletion();
            DisplayRecentAssignments();
            DisplayTopPerformanceStandardsAndAssessmentTypes();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Displays statistics for the user's coursework completion
        /// </summary>
        private void DisplayCourseworkCompletion()
        {
            // Load the assignment database
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Initialise bounds to seperate each assessment weight class
            int formativeBound = 0;
            int minorBound = 10;
            int moderateBound = 25;

            // Initialise collections for each assessment weight class
            List<List<string>> formativeAssignments = assignmentDatabase.Where(x => Convert.ToInt16(x[(int)AProp.Weight]) <= formativeBound).ToList();
            List<List<string>> minorAssignments = assignmentDatabase.Where(x => formativeBound < Convert.ToInt16(x[(int)AProp.Weight]) && Convert.ToInt16(x[(int)AProp.Weight]) <= minorBound).ToList();
            List<List<string>> moderateAssignments = assignmentDatabase.Where(x => minorBound < Convert.ToInt16(x[(int)AProp.Weight]) && Convert.ToInt16(x[(int)AProp.Weight]) <= moderateBound).ToList();
            List<List<string>> majorAssignments = assignmentDatabase.Where(x => moderateBound < Convert.ToInt16(x[(int)AProp.Weight])).ToList();

            // Count the number of completed assignments overall, and for each weight class
            double completedAssignments = assignmentDatabase.Where(x => DateTime.Parse(x[(int)AProp.DueDate]) < DateTime.Now).Count();
            double completedFormativeAssignments = formativeAssignments.Where(x => DateTime.Parse(x[(int)AProp.DueDate]) < DateTime.Now).Count();
            double completedMinorAssignments = minorAssignments.Where(x => DateTime.Parse(x[(int)AProp.DueDate]) < DateTime.Now).Count();
            double completedModerateAssignments = moderateAssignments.Where(x => DateTime.Parse(x[(int)AProp.DueDate]) < DateTime.Now).Count();
            double completedMajorAssignments = majorAssignments.Where(x => DateTime.Parse(x[(int)AProp.DueDate]) < DateTime.Now).Count();

            // Calculate the percentage of completed assignments, throughout the year, for each weight class, roudning the total completion to 2 decimal places and the rest to 0 (integer) decimal places
            CourseworkCompletion = Math.Round((completedAssignments / assignmentDatabase.Count * 100), 2).ToString() + "%";
            FormativeCourseworkCompletion = Math.Round(completedFormativeAssignments / formativeAssignments.Count * 100).ToString() + "% Formative";
            MinorCourseworkCompletion = Math.Round(completedMinorAssignments / minorAssignments.Count * 100).ToString() + "% Minor";
            ModerateCourseworkCompletion = Math.Round(completedModerateAssignments / moderateAssignments.Count * 100).ToString() + "% Moderate";
            MajorCourseworkCompletion = Math.Round(completedMajorAssignments / majorAssignments.Count * 100).ToString() + "% Major";
        }

        /// <summary>
        /// Finds some number of soonest, incomplete assignments which are due and displays them
        /// </summary>
        private void DisplayRecentAssignments()
        {
            // The number of assignments to find
            int displayedAssignments = 5;

            // Load the assignment database and order it by due date
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();
            assignmentDatabase = assignmentDatabase.OrderBy(x => DateTime.Parse(x[(int)AProp.DueDate])).ToList();

            // Initialise a list of indicies to remove from this database, when looking for expired assignments
            List<int> removedAssignmentIndices = new List<int>();
            
            // For each assignment in the assginment database...
            foreach (int i in Enumerable.Range(0, assignmentDatabase.Count))
            {
                // If the assignment's due date has passed...
                if (DateTime.Parse(assignmentDatabase[i][(int)AProp.DueDate]) < DateTime.Now)
                {
                    // Book this assignment's index to be removed
                    removedAssignmentIndices.Add(i);
                }
            }

            // Order the removed index keys in descending order
            removedAssignmentIndices = removedAssignmentIndices.OrderByDescending(x => x).ToList();

            // Remove all the expired assignments from the assignment database
            foreach (int i in removedAssignmentIndices)
            {
                assignmentDatabase.RemoveAt(i);
            }

            // Select the x soonest assignments which have not expired from the assignment database
            List<List<string>> selectedAssignments = assignmentDatabase.Take(displayedAssignments).ToList();

            // Display the x soonest assignments
            foreach (List<string> assignment in selectedAssignments)
            {
                DisplayAssignment(assignment);
            }
        }

        /// <summary>
        /// Displays a assignment on the view of most recent assignent
        /// </summary>
        /// <param name="assignment">The assignment data</param>
        private void DisplayAssignment(List<string> assignment)
        {
            // Truncate the displayed name
            string displayName = assignment[(int)AProp.AssessmentType] + ": " + assignment[(int)AProp.Name];
            displayName = ValidationHelpers.TruncateString(displayName);

            // Create a new assignment with all the nessecary parameters
            StudentAssignmentItemViewModel addedAssignment = new StudentAssignmentItemViewModel()
            {
                Course = assignment[(int)AProp.Course],
                AssessmentType = assignment[(int)AProp.AssessmentType],
                Name = assignment[(int)AProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindAssignmentIcon(assignment),
                DueDate = assignment[(int)AProp.DueDate],
                Properties = FindAssignmentProperties(assignment)
            };

            // Add this assignment to the collection of assignmnets
            MostRecentAssignments.Add(addedAssignment);
        }

        /// <summary>
        /// [LINKED] Finds an assignment's properties relative to the other assignments in its course.
        /// </summary>
        /// <param name="assignment">The assignment with discoverable properties</param>
        /// <returns>The assignment's collection of property badges</returns>
        public ObservableCollection<PropertyBadgeViewModel> FindAssignmentProperties(List<string> assignment)
        {
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

        /// <summary>
        /// Displays the performance standards and assessment types most relevant to the user (most frequent in their assignments)
        /// </summary>
        public void DisplayTopPerformanceStandardsAndAssessmentTypes()
        {
            // The number of most important performance standards and assessment types to display
            int displayedBadges = 4;

            // Load the assignment database
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Initialise lists for the raw dataset of performance standards and assessment types to load into
            List<string> rawPerformanceStandards = new List<string>();
            List<string> rawAssessmentTypes = new List<string>();

            // For each assignment in the assignment database...
            foreach (List<string> assignment in assignmentDatabase)
            {
                // Load the assignment's assessment type and all of its performance standards into the raw lists
                rawPerformanceStandards.AddRange(assignment[(int)AProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Remove(x.Length - 1)).ToList());
                rawAssessmentTypes.Add(assignment[(int)AProp.AssessmentType]);
            }

            // Sort the performance standards and assessment types by their frequency, then reduce them to a one-hot vector
            List<string> performanceStandardDistribution = rawPerformanceStandards.GroupBy(i => i).OrderByDescending(g => g.Count()).Select(g => g.Key).ToList();
            List<string> assessmentTypeDistribution = rawAssessmentTypes.GroupBy(i => i).OrderByDescending(g => g.Count()).Select(g => g.Key).ToList();

            // Take the specified number of performance standards and assessment types from this list
            List<string> selectedPerformanceStandards = performanceStandardDistribution.Take(displayedBadges).ToList();
            List<string> selectedAssignmentTypes = assessmentTypeDistribution.Take(displayedBadges).ToList();

            // For each selected repormance standard...
            foreach (string standard in selectedPerformanceStandards)
            {
                // Display the performance standard as a badge
                AspectBadgeViewModel addedStandard = new AspectBadgeViewModel { Code = standard, RootCode = standard };
                ImportantPerformanceStandards.Add(addedStandard);
            }

            // For each selected assessment type...
            foreach (string type in selectedAssignmentTypes)
            {
                // Display the assessment type as a badge
                AspectBadgeViewModel addedType = new AspectBadgeViewModel { Code = type, RootCode = type };
                ImportantAssessmentTypes.Add(addedType);
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
