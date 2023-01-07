using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using static SACEology.ValidationHelpers;
using Newtonsoft.Json;

namespace SACEology.ViewModel
{
    class StudentMyResultsPageViewModel : BaseViewModel
    {
        #region Public Properties (Universal)

        /// <summary>
        /// A list of possible calculation types (calculator modes) for this page.
        /// </summary>
        public List<string> CalculationTypes { get; set; } = new List<string> { "Overall Performance", "Course Performance" };

        /// <summary>
        /// A private variable storing the selected calculation type
        /// </summary>
        private string _selectedCalculationType { get; set; } = "Overall Performance";

        /// <summary>
        /// The selected calculation type.
        /// </summary>
        public string SelectedCalculationType
        {
            get
            {
                return _selectedCalculationType;
            }
            set
            {
                _selectedCalculationType = value;
                DisplaySavedCourses();

                Settings.Default.SelectedOverallPerformance = value == "Overall Performance";
                IsOverallPerformance = value == "Overall Performance";
                IsCoursePerformance = value == "Course Performance";
            }
        }

        /// <summary>
        /// Whether this page is currently showing overall performance calculation
        /// </summary>
        public bool IsOverallPerformance { get; set; } = true;

        /// <summary>
        /// Whether this page is currently showing course performance calculation
        /// </summary>
        public bool IsCoursePerformance { get; set; } = false;

        #endregion

        #region Public Properties (Overall Performance)

        /// <summary>
        /// An observable collection of the user's current courses.
        /// </summary>
        public ObservableCollection<StudentCourseInputViewModel> Courses { get; set; } = new ObservableCollection<StudentCourseInputViewModel>();

        public string PredictedScaling { get; set; } = "N/A";

        public string BonusPoints { get; set; } = "N/A";

        public string PredictedATAR { get; set; } = "N/A";

        #endregion

        #region Public Variables (Course Performance)

        /// <summary>
        /// A list of of the user's course names.
        /// </summary>
        public List<string> CourseNames { get; set; } = DatabaseHelpers.LoadCourseDatabase().Select(x => x[(int)CProp.Name]).ToList();

        /// <summary>
        /// A private variable storing the selected course.
        /// </summary>
        private string _selectedCourse { get; set; } = DatabaseHelpers.LoadCourseDatabase().Count > 0 ? DatabaseHelpers.LoadCourseDatabase().Select(x => x[(int)CProp.Name]).ToList()[0] : string.Empty;

        /// <summary>
        /// The selected course.
        /// </summary>
        public string SelectedCourse
        {
            get
            {
                return _selectedCourse;
            }
            set
            {
                _selectedCourse = value;

                // Save the currently selected course
                Settings.Default.SelectedCourse = _selectedCourse;
                Settings.Default.Save();

                // Reset the grade calculation variables
                ResetGradeCalculationVariables();

                // Display the course's assignments
                DisplayCourseAssignments();
            }
        }

        /// <summary>
        /// A list of all assignments for the currently selected course.
        /// </summary>
        public ObservableCollection<StudentAssignmentInputViewModel> Assignments { get; set; } = new ObservableCollection<StudentAssignmentInputViewModel>();

        /// <summary>
        /// A private variable storing a list of this course's performance standards.
        /// </summary>
        private List<string> _standards { get; set; }

        /// <summary>
        /// A list of this course's performance standards.
        /// </summary>
        public List<string> Standards
        {
            get
            {
                return _standards;
            }
            set
            {
                _standards = value;
                IsGradeBreakdownEnabled = _standards.Count > 0;
            }
        }

        /// <summary>
        /// A list encoding of grade sums of performance standards within this course.
        /// </summary>
        public List<double> Sums { get; set; }

        /// <summary>
        /// A list encoding of occurences of performance standards within this course.
        /// </summary>
        public List<double> Occurences { get; set; }

        /// <summary>
        /// A list encoding of grades achieved for performance standards within this course.
        /// </summary>
        public List<double> Grades { get; set; }

        /// <summary>
        /// The user's grade for this course if the assignment's weights were exclusively considered.
        /// </summary>
        public string PureWeightGrade { get; set; }

        /// <summary>
        /// The user's grade for this course as an average of their grades for its performance standards
        /// </summary>
        public string PerformanceStandardGrade { get; set; }

        /// <summary>
        /// Whether a grade breakdown can be enabled (disabled if nothing has been entered)
        /// </summary>
        public bool IsGradeBreakdownEnabled { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the "?" button is pressed, to view a pop up showing the breakdown of a performance standard grade
        /// </summary>
        public ICommand ViewGradeBreakdownButtonCommand { get; set; }

        public ICommand ViewCourseGoalSeekerButtonCommand { get; set; }

        #endregion

        #region Constructor

        public StudentMyResultsPageViewModel()
        {
            // Initialise button relay commands
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
            ViewGradeBreakdownButtonCommand = new RelayCommand(() => ShowGradeBreakdownPopUp());
            ViewCourseGoalSeekerButtonCommand = new RelayCommand(() => ShowCourseGoalSeekerPopUp());

            // If there is a default course to display...
            if (DatabaseHelpers.LoadCourseDatabase().Count > 0)
            {
                // Display the assignments for the default course
                ResetGradeCalculationVariables();
                DisplayCourseAssignments();
            }

            // Hook into external events relevant to the grade adjustment
            ValidationAggregator.OnGradeEdited += DisplayPureWeightGrade;
            ValidationAggregator.OnGradeEdited += DisplayPerformanceStandardGrade;

            ValidationAggregator.OnGradeEdited += DisplayPredictedScaling;
            ValidationAggregator.OnGradeEdited += DisplayBonusPoints;
            ValidationAggregator.OnGradeEdited += DisplayPredictedATAR;

            DisplaySavedCourses();
        }

        #endregion

        #region Private Helpers (Overall Calculation)

        /// <summary>
        /// Displays all assignment inputs within this course.
        /// </summary>
        private void DisplaySavedCourses()
        {
            // Clear the list of assignments
            Courses.Clear();

            // Load the assignment database
            List<List<string>> courseDatabase = DatabaseHelpers.LoadCourseDatabase();

            // For each assignment in this course
            foreach (List<string> course in courseDatabase)
            {
                // Display the assignment as a grade input
                DisplayCourse(course);
            }
        }

        /// <summary>
        /// Displays a course input by building its viewmodel and adding it to the observable collection.
        /// </summary>
        /// <param name="course">The assignment input</param>
        private void DisplayCourse(List<string> course)
        {
            // Truncate the displayed name
            string displayName = course[(int)CProp.SubjectCode] + ": " + course[(int)CProp.Name];
            displayName = TruncateString(displayName);

            // Load the subject database
            List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();

            // Load the course's scaling score if the user wants their ATAR grades to be calculated with scaling
            string scalingScore = Settings.Default.ScaleATARGrades ? course[(int)CProp.ScalingScore] : "0";

            // Create a new course with all the nessecary parameters
            StudentCourseInputViewModel addedCourse = new StudentCourseInputViewModel()
            {
                Code = course[(int)CProp.SubjectCode],
                Name = course[(int)CProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindCourseIcon(course),
                ScalingScore = scalingScore
            };

            // Add this course to the collection of assignments
            Courses.Add(addedCourse);
        }

        /// <summary>
        /// Displays the student's predicted scaling, for the four grades that go toward their ATAR.
        /// </summary>
        private void DisplayPredictedScaling()
        {
            // Take the student's four best courses, the ones which go towards their ATAR
            List<StudentCourseInputViewModel> ATARCourses = Courses.ToList().OrderByDescending(x => x.Grade).Take(4).ToList();

            // Initialise a new double for the sum of the student's scaling scores
            double scalingSum = 0;

            // For each the student's top four courses...
            foreach (StudentCourseInputViewModel course in ATARCourses)
            {
                scalingSum += Convert.ToDouble(course.ScalingScore);
            }

            // Display the user's predicted scaling
            PredictedScaling = (scalingSum / 4).ToString() + "%";
        }

        private void DisplayBonusPoints()
        {
            // Take the student's four best courses, the ones which go towards their ATAR
            List<StudentCourseInputViewModel> ATARCourses = Courses.ToList().OrderByDescending(x => x.Grade).Take(4).ToList();

            // Initialise a list of subject codes that yield bonus points
            List<string> bonusPointSubjectCodes = new List<string> { "ELS", "MHS", "MSC", "ITC", "JAC" };

            // Initialise a list of suibjets with exams, and load these subjects into the list
            List<string> examinableSubjectCodes = new List<string>();

            List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();

            foreach (List<string> subject in subjectDatabase)
            {
                List<string> assessmentTypes = subject[(int)SProp.AssessmentTypes].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (assessmentTypes.Contains("EXAM"))
                {
                    examinableSubjectCodes.Add(subject[(int)SProp.SubjectCode]);
                }
            }

            // Initialise a new double to store the student's bonus points
            double bonusPoints = 0;

            // For each of the student's top four courses...
            foreach (StudentCourseInputViewModel course in ATARCourses)
            {
                // If this course counts towards 
                if (bonusPoints < 4 && bonusPointSubjectCodes.Contains(course.Code))
                {
                    // Add two bonus points to the student's ATAR
                    bonusPoints += 2;
                }
            }

            // Deduct two points from the student's bonus points as a base for having 0 exams
            bonusPoints -= 2;

            // For each of the student's top four courses...
            foreach (StudentCourseInputViewModel course in ATARCourses)
            {
                // If this course has an exam...
                if (examinableSubjectCodes.Contains(course.Code))
                {
                    // Add a bonus point to the student's ATAR
                    bonusPoints++;
                }
            }

            BonusPoints = bonusPoints.ToString();
        }

        /// <summary>
        /// Display's the user's predicted ATAR based on their four best subjects.
        /// </summary>
        private void DisplayPredictedATAR()
        {
            // Take the student's four best courses, the ones which go towards their ATAR
            List<StudentCourseInputViewModel> ATARCourses = Courses.ToList().OrderByDescending(x => x.Grade).Take(4).ToList();

            // Initialise a new double for the sum of the student's ATAR grades
            double gradeSum = 0;

            // For each the student's top four courses...
            foreach (StudentCourseInputViewModel course in ATARCourses)
            {
                // Add the current course's weight, times its scaling score, to the sum of the student's ATAR grades
                gradeSum += Convert.ToDouble(course.Grade) * (1 + Convert.ToDouble(Convert.ToDouble(course.ScalingScore)));
            }

            // The student's raw ATAR is the average of these grades, normalised between 0 and 100
            double rawATAR = (gradeSum / 4 / 15) * 100;

            // Add the bonus points to the student's ATAR and round it to two decimal places
            double finalATAR = Math.Round(rawATAR + Convert.ToDouble(BonusPoints), 2);

            // Clamp the student's final ATAR between 0 and 99.95 and display it
            PredictedATAR = Math.Min(Math.Max(finalATAR, 0), 99.95).ToString();
        }

        #endregion

        #region Private Helpers (Course Calculation)

        /// <summary>
        /// Displays all assignment inputs within this course.
        /// </summary>
        private void DisplayCourseAssignments()
        {
            // Clear the list of assignments
            Assignments.Clear();

            // Load the assignment database
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Find all assignments for the currently selected course
            List<List<string>> assignments = assignmentDatabase.Where(x => x[(int)AProp.Course] == SelectedCourse).ToList();

            // For each assignment in this course
            foreach (List<string> assignment in assignments)
            {
                // Display the assignment as a grade input
                DisplayAssignment(assignment);
            }
        }

        /// <summary>
        /// Displays an assignment input by building its viewmodel and adding it to the observable collection.
        /// </summary>
        /// <param name="assignment">The assignment input</param>
        private void DisplayAssignment(List<string> assignment)
        {
            // Truncate the displayed name
            string displayName = assignment[(int)AProp.AssessmentType] + ": " + assignment[(int)AProp.Name];
            displayName = TruncateString(displayName);

            // Create a new assignment with all the nessecary parameters
            StudentAssignmentInputViewModel addedAssignment = new StudentAssignmentInputViewModel()
            {
                AssessmentType = assignment[(int)AProp.AssessmentType],
                Course = assignment[(int)AProp.Course],
                Name = assignment[(int)AProp.Name],
                DisplayName = displayName,
                Icon = DatabaseHelpers.FindAssignmentIcon(assignment),
                Weight = assignment[(int)AProp.Weight],
                IsReadOnly = false
            };

            // Add this assignment to the collection of assignmnets
            Assignments.Add(addedAssignment);
        }

        /// <summary>
        /// Displays the student's grade for their selected course, based on the current, raw results of their completed assignments.
        /// </summary>
        private void DisplayPureWeightGrade()
        {
            // Initialise a variable to store the user's number of graded assignments and sum of graded scores
            double gradeSum = 0;
            double weightSum = 0;

            // For every assginment in the currently selected course...
            foreach (StudentAssignmentInputViewModel assignment in Assignments)
            {
                // Initialise a variable as the assignment's grade
                string grade = assignment.Grade;

                // If the assignment has a grade entered...
                if (!string.IsNullOrEmpty(grade))
                {
                    // Increment the grade count and add this grade to the overall sum
                    weightSum += Convert.ToDouble(assignment.Weight) / 100;
                    gradeSum += Convert.ToDouble(grade) * (Convert.ToDouble(assignment.Weight) / 100);
                }

                // Calculate both representations for the pure weight grade
                double numericGrade = Math.Round(gradeSum * 1 / weightSum, 2);

                // Display the user's pure weight grade
                PureWeightGrade = NumericToDisplayGrade(numericGrade);
            }
        }

        /// <summary>
        /// Displays the user's performance standard-based grade for this course, and their weakest area
        /// </summary>
        private void DisplayPerformanceStandardGrade()
        {
            // Find the performance standards for this subject's course
            Standards = DatabaseHelpers.FindSubjectPerformanceStandards(SelectedCourse);

            // Find the grade sums and counts for the performance standards following this course's assessment
            Sums = FindPerformanceStandardGradeSums(Standards);
            Occurences = FindPerformanceStandardOccurences(Standards);

            if (Sums.Sum() > 0)
            {
                // Calculate the grades for each performance standard following this course's assessment
                Grades = FindPerformanceStandardGrades(Standards, Sums, Occurences);

                // Calculate both representations for the pure weight grade, excluding performance standards that have no assignments attributed to them
                double numericGrade = Math.Round(Grades.Sum() / Occurences.Where(x => x > 0).Count(), 2);

                // Display the user's performance standard grade
                PerformanceStandardGrade = NumericToDisplayGrade(numericGrade);
            }
            else
            {
                ResetGradeCalculationVariables();
            }
        }

        /// <summary>
        /// Finds the total scores for each performance standard in a course.
        /// </summary>
        /// <param name="subjectPerformanceStandards">The course's performance standards</param>
        /// <returns>The total scores for this course's performance standards</returns>
        private List<double> FindPerformanceStandardGradeSums(List<string> subjectPerformanceStandards)
        {
            // Load the assignment, course and subject databases
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Initialise two lists of zeroes, equal in length to the number of performance standards), to store a running total for the grade sums for each performance standard, and a running total of each performance standard's occurences
            List<double> performanceStandardGradeSums = new List<double>();
            performanceStandardGradeSums.AddRange(Enumerable.Repeat(0.0, subjectPerformanceStandards.Count));

            // For each assignment in the currently selected course's list of assignments
            foreach (StudentAssignmentInputViewModel assignment in Assignments)
            {
                // Determine the assignment's grade as a string value
                string grade = assignment.Grade;

                // If the assignment is not marked as incomplete but does have a grade entered...
                if (!(grade == "I") && !string.IsNullOrEmpty(grade))
                {
                    // Initialise a list of strings to store this assignment's performance standards
                    List<string> performanceStandards = new List<string>();

                    // For each assignment in the assignment database...
                    foreach (List<string> a in assignmentDatabase)
                    {
                        // If the assignment's course and name match with the current assignment's course and name
                        if (a[(int)AProp.Course] == assignment.Course && a[(int)AProp.Name] == assignment.Name)
                        {
                            // Use these properties to determine the current assignment's performance standards, eliminating empty entries
                            performanceStandards = a[(int)AProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x)).ToList();
                        }
                    }

                    // For each standard in this assignment's list of performance standards...
                    foreach (string standard in performanceStandards)
                    {
                        // Initialise standardIndex as the index position (on the subject performance standard list) that the current performance standard falls
                        int standardIndex = subjectPerformanceStandards.FindIndex(a => a == standard);

                        // If the performance standard is not found, -1 is returned as an index. To protect the code, this should automatically become 0 [IMPLEMENT ERROR]
                        standardIndex = standardIndex < 0 ? 0 : standardIndex;

                        // Add this assignment's grade to its performance standard's indexed position on the overall performance standard grade sum matrix
                        performanceStandardGradeSums[standardIndex] += Convert.ToDouble(grade);
                    }
                }
            }

            return performanceStandardGradeSums;
        }

        /// <summary>
        /// Finds the number of occurences for each performance standard in a course.
        /// </summary>
        /// <param name="subjectPerformanceStandards">The course's performance standards</param>
        /// <returns>The number of occurences for this course's performance standards</returns>
        private List<double> FindPerformanceStandardOccurences(List<string> subjectPerformanceStandards)
        {
            // Load the assignment, course and subject databases
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Initialise two lists of zeroes, equal in length to the number of performance standards), to store a running total for the grade sums for each performance standard, and a running total of each performance standard's occurences
            List<double> performanceStandardGradeCounts = new List<double>();
            performanceStandardGradeCounts.AddRange(Enumerable.Repeat(0.0, subjectPerformanceStandards.Count));

            // For each assignment in the currently selected course's list of assignments
            foreach (StudentAssignmentInputViewModel assignment in Assignments)
            {
                // Determine the assignment's grade as a string value
                string grade = assignment.Grade;

                // If the assignment is not marked as incomplete but does have a grade entered...
                if (!(grade == "I") && !string.IsNullOrEmpty(grade))
                {
                    // Initialise a list of strings to store this assignment's performance standards
                    List<string> performanceStandards = new List<string>();

                    // For each assignment in the assignment database...
                    foreach (List<string> a in assignmentDatabase)
                    {
                        // If the assignment's course and name match with the current assignment's course and name
                        if (a[(int)AProp.Course] == assignment.Course && a[(int)AProp.Name] == assignment.Name)
                        {
                            // Use these properties to determine the current assignment's performance standards, eliminating empty entries
                            performanceStandards = a[(int)AProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x)).ToList();
                        }
                    }

                    // For each standard in this assignment's list of performance standards...
                    foreach (string standard in performanceStandards)
                    {
                        // Initialise standardIndex as the index position (on the subject performance standard list) that the current performance standard falls
                        int standardIndex = subjectPerformanceStandards.FindIndex(a => a == standard);

                        // If the performance standard is not found, -1 is returned as an index. To protect the code, this should automatically become 0 [IMPLEMENT ERROR]
                        standardIndex = standardIndex < 0 ? 0 : standardIndex;

                        // Add this assignment's grade to its performance standard's indexed position on the overall performance standard grade sum matrix
                        performanceStandardGradeCounts[standardIndex]++;
                    }
                }
            }

            return performanceStandardGradeCounts;
        }

        /// <summary>
        /// Finds the grade for each performance standard in a course.
        /// </summary>
        /// <param name="subjectPerformanceStandards">The course's performance standards</param>
        /// <param name="performanceStandardGradeSums">The total scores for this course's performance standards</param>
        /// <param name="performanceStandardCounts">The number of occurences for this course's performance standards</param>
        /// <returns>The grades for this course's performance standards</returns>
        private List<double> FindPerformanceStandardGrades(List<string> subjectPerformanceStandards, List<double> performanceStandardGradeSums, List<double> performanceStandardCounts)
        {
            // Intialise a new list of zeros, equal in length to the number of this course's performance stnadards, to show performance standard grades
            List<double> performanceStandardGrades = new List<double>();
            performanceStandardGrades.AddRange(Enumerable.Repeat(0.0, subjectPerformanceStandards.Count));

            // For each integer in a list of integers equal to the number of performance standards
            foreach (int i in Enumerable.Range(0, performanceStandardCounts.Count))
            {
                // If there is at least assignment in this course with the ith performance standard...
                if (performanceStandardCounts[i] > 0)
                {
                    // Calculate the average grade achaived for this performance standard and save it to the list
                    performanceStandardGrades[i] = performanceStandardGradeSums[i] / performanceStandardCounts[i];
                }
            }

            return performanceStandardGrades;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Resets all varirables for grade calculation.
        /// </summary>
        private void ResetGradeCalculationVariables()
        {
            Standards = new List<string>();
            Sums = new List<double>();
            Occurences = new List<double>();
            Grades = new List<double>();

            PureWeightGrade = "N/A";
            PerformanceStandardGrade = "N/A";
        }

        #endregion

        #region Pop-Up Core

        /// <summary>
        /// Shows the "Grade Breakdown" pop up.
        /// </summary>
        private void ShowGradeBreakdownPopUp()
        {
            // Find the performance standards for this subject's course
            List<string> subjectPerformanceStandards = DatabaseHelpers.FindSubjectPerformanceStandards(SelectedCourse);

            // Find the grade sums and counts for the performance standards following this course's assessment
            List<double> performanceStandardGradeSums = FindPerformanceStandardGradeSums(subjectPerformanceStandards);
            List<double> performanceStandardCounts = FindPerformanceStandardOccurences(subjectPerformanceStandards);

            // Calculate the grades for each performance standard following this course's assessment
            List<double> performanceStandardGrades = FindPerformanceStandardGrades(subjectPerformanceStandards, performanceStandardGradeSums, performanceStandardCounts);

            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastGradeBreakdownPopUpCreation(subjectPerformanceStandards, performanceStandardCounts, performanceStandardGrades);
            }
        }

        /// <summary>
        /// Shows the "Goal Seeker (Course)" pop up.
        /// </summary>
        private void ShowCourseGoalSeekerPopUp()
        {
            string serializedAssignments = JsonConvert.SerializeObject(Assignments);
            Settings.Default.StudentMyResultsAssignments = serializedAssignments;
            Settings.Default.Save();

            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastCourseGoalSeekerPopUpCreation();
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
