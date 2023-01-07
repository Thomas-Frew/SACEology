using SACEology.ViewModel.Base;
using System.Windows.Input;
using SACEology;
using System.Collections.Generic;
using SACEology.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using SACEology.Properties;
using System;
using static SACEology.ValidationHelpers;
using Newtonsoft.Json;

class StudentCourseGoalSeekerPopUpViewModel : BaseViewModel
{
    #region Public Properties

    public string SelectedCourse = Settings.Default.SelectedCourse;

    public ObservableCollection<StudentAssignmentInputViewModel> InputAssignments { get; set; } = JsonConvert.DeserializeObject< ObservableCollection<StudentAssignmentInputViewModel>>(Settings.Default.StudentMyResultsAssignments);

    public ObservableCollection<StudentAssignmentInputViewModel> OutputAssignments { get; set; } = new ObservableCollection<StudentAssignmentInputViewModel>();

    /// <summary>
    /// A private variable storing the grade the student wants to achaive in their course overall.
    /// </summary>
    public string _gradeGoal { get; set; }

    /// <summary>
    /// The grade the student wants to achaive in their course overall.
    /// </summary>
    public string GradeGoal
    {
        get
        {
            return _gradeGoal;
        }
        set
        {
            _gradeGoal = ValidateGrade(value);
            GoalEntered = !string.IsNullOrEmpty(value);

            CalculateGoalContextual();
        }
    }

    public bool GoalEntered { get; set; }

    /// <summary>
    /// A list of this course's performance standards.
    /// </summary>
    public List<string> Standards { get; set; }

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

    private GoalSeekerCalculation _calculationType { get; set; } = GoalSeekerCalculation.MostEfficient;

    /// <summary>
    /// The currently selected goal seeker calculation type.
    /// </summary>
    public GoalSeekerCalculation CalculationType
    {
        get
        {
            return _calculationType;
        }
        set
        {
            _calculationType = value;
            CalculateGoalContextual();
        }
    }

    /// <summary>
    /// Whether the student's grade goal is achievable.
    /// </summary>
    public bool IsGradeGoalAchievable { get; set; } = false;

    /// <summary>
    /// Whether the student's grade goal is unachaivable.
    /// </summary>
    public bool IsGradeGoalUnachievable { get; set; } = false;

    #endregion

    #region

    /// <summary>
    /// The command for when the left arrow button is pressed, to select the previous calculation type.
    /// </summary>
    public ICommand DecrementCalculationTypeCommand { get; set; }

    /// <summary>
    /// The command for when the right arrow button is pressed, to select the next calculation type.
    /// </summary>
    public ICommand IncrementCalculationTypeCommand { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public StudentCourseGoalSeekerPopUpViewModel()
    {
        DecrementCalculationTypeCommand = new RelayCommand(() => DecrementCalculationType());
        IncrementCalculationTypeCommand = new RelayCommand(() => IncrementCalculationType());

        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());

        ValidationAggregator.OnGradeEdited += CalculateGoalContextual;

    }

    #endregion

    #region Private Helpers

    #region Mode Toggling Core

    /// <summary>
    /// Switches to the next goal seeker calculation type.
    /// </summary>
    private void IncrementCalculationType()
    {
        CalculationType = (GoalSeekerCalculation)(Convert.ToInt16(CalculationType) + 1);

        if (CalculationType > GoalSeekerCalculation.AntiFolio)
        {
            CalculationType = GoalSeekerCalculation.MostEfficient;
        }
    }

    /// <summary>
    /// Switches to the previous goal seeker calculation type.
    /// </summary>
    private void DecrementCalculationType()
    {
        CalculationType = (GoalSeekerCalculation)(Convert.ToInt16(CalculationType) - 1);

        if (CalculationType < GoalSeekerCalculation.MostEfficient)
        {
            CalculationType = GoalSeekerCalculation.AntiFolio;
        }
    }

    #endregion

    #region Assignment Display Core

    /// <summary>
    /// Finds all incomplete assignments from the list of assignment inputs
    /// </summary>
    /// <returns>A list of incomplete assignments</returns>
    private List<StudentAssignmentInputViewModel> FindIncompleteAssignments()
    {
        // Initialise a list to store incomplete assignments
        return CloneAssignmentInputs(InputAssignments.ToList()).Where(x => string.IsNullOrEmpty(x.Grade)).ToList();
    }

    /// <summary>
    /// Displays an assignment in the output
    /// </summary>
    /// <param name="assignment"></param>
    /// <param name="grade"></param>
    private void DisplayAssignment(StudentAssignmentInputViewModel assignment, double grade)
    {
        // Create a new assignment with all the nessecary parameters
        StudentAssignmentInputViewModel addedAssignment = new StudentAssignmentInputViewModel
        {
            Course = assignment.Course,
            Name = assignment.Name,
            DisplayName = assignment.DisplayName,
            Icon = assignment.Icon,
            AssessmentType = assignment.AssessmentType,
            Weight = assignment.Weight,
            Grade = grade.ToString(),
            IsReadOnly = true
        };

        // Add this assignment to the collection of assignmnets
        OutputAssignments.Add(addedAssignment);
    }

    #endregion 
    
   /// <summary>
   /// Calculates the user's grade required for each incomplete assignment to achieve their overall grade goal, depending on the calculation type they selected.
   /// </summary>
    private void CalculateGoalContextual()
    {
        // If the user has, in fact, entered a grade goal...
        if (!string.IsNullOrEmpty(GradeGoal))
        {
            // Find whether the user's grade goal is achaivable
            IsGradeGoalAchievable = true;
            IsGradeGoalUnachievable = false;

            // Clear the collection of output assignments
            OutputAssignments.Clear();

            switch (CalculationType)
            {
                // If the user wants to achieve their grade goal as efficiently as possible, calculate their required grades as such
                case GoalSeekerCalculation.MostEfficient:
                    CalculateMostEfficientAssignmentGrades();
                    break;

                // If the user wants to guarentee getting their grade goal, with both kinds of grade calculation, calculate their required grades as such
                case GoalSeekerCalculation.GuaranteedEntry:
                    CalculateGuaranteedAssignmentGrades();
                    break;

                // If the user wants to reach their grade goal as soon as possible, calculate their required grades as such
                case GoalSeekerCalculation.FastestActionable:
                    CalculateFastestActionableAssignmentGrades();
                    break;

                // If the user wants to reach their grade goal as late as possible, calculate their required grades as such
                case GoalSeekerCalculation.SlowestActionable:
                    CalculateSlowestActionableAssignmentGrades();
                    break;

                // If the user wants to reach their grade goal while avoiding exams as much as possible, calculate their required grades as such
                case GoalSeekerCalculation.AntiExam:
                    CalculateAntiExamAssignmentGrades();
                    break;

                // If the user wants to reach their grade goal while avoiding folios as much as possible, calculate their required grades as such
                case GoalSeekerCalculation.AntiFolio:
                    CalculateAntiFolioAssignmentGrades();
                    break;
            }
        }
        else
        {
            // [IMPLEMENT ERROR]
        }

        Console.WriteLine("Success");
    }

    /// <summary>
    /// Calculates the most efficient way to the user to achieve their grade goals.
    /// </summary>
    private void CalculateMostEfficientAssignmentGrades()
    {
        // Finds a list of assignments which havent been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        // Find the average score needed to make up for the user's missing points
        double pureWeightGradeGoal = FindPureWeightGradeGoal();

        // If the student has to acheive more than an A+ to reach their grade goal...
        if (pureWeightGradeGoal > 15)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
        else
        {
            // For each missing assignment, display this assignment as an output with the new score
            foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
            {
                DisplayAssignment(assignment, Math.Round(pureWeightGradeGoal, 2));
            }
        }
    }

    /// <summary>
    /// Calculates the grades the user should achieve in their assignments, to be guaranteed to achaive it with both kinds of grade calculation.
    /// </summary>
    private void CalculateGuaranteedAssignmentGrades()
    {
        // Finds a list of assignments which havent been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        double pureWeightGradeGoal = FindPureWeightGradeGoal();
        double performanceStandardGradeGoal = FindPerformanceStandardGradeGoal();

        double guaranteedGradeGoal = Math.Max(pureWeightGradeGoal, performanceStandardGradeGoal);

        // If the student has to acheive more than an A+ to reach their grade goal...
        if (guaranteedGradeGoal > 15)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
        else
        {
            // For each missing assignment, display this assignment as an output with the new score
            foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
            {
                DisplayAssignment(assignment, Math.Round(guaranteedGradeGoal, 2));
            }
        }
    }

    /// <summary>
    /// Calculates the grades the user should achieve in their assignments, to be guarenteed to achaive it with both kinds of grade calculation, plus a grade band above.
    /// </summary>
    private void CalculateSafeOutcomeAssignmentGrades()
    {
        // Finds a list of assignments which havent been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        double pureWeightGradeGoal = FindPureWeightGradeGoal();
        double performanceStandardGradeGoal = FindPerformanceStandardGradeGoal();

        double guaranteedGradeGoal = Math.Max(pureWeightGradeGoal, performanceStandardGradeGoal);
        double safeOutcomeGradeGoal = guaranteedGradeGoal + 1;

        // If the student has to acheive more than an A+ to reach their grade goal...
        if (safeOutcomeGradeGoal > 15)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
        else
        {
            // For each missing assignment, display this assignment as an output with the new score
            foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
            {
                DisplayAssignment(assignment, Math.Round(safeOutcomeGradeGoal, 2));
            }
        }

    }

    /// <summary>
    /// Calculates the grades the user should achieve in their assignments, to achieve their grade goals as soon as possible.
    /// </summary>
    private void CalculateFastestActionableAssignmentGrades()
    {
        // Finds a list of assignments which havent been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        // Finds how many points the user needs to acheive to read their overall grade goal.
        double missingPoints = FindMissingPoints();

        // For each missing assignment, display this assignment as an output with the new score
        foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
        {
            // Initialise a variable containing as many possible points left can be awarded to the current assignment
            double awardedPoints = Math.Max(Math.Min(missingPoints, 15), 1);

            // Display the current assignment with the current number of missing points
            DisplayAssignment(assignment, Math.Round(awardedPoints, 2));

            // Remove the awarded number of missing points from the missing points pool
            missingPoints -= awardedPoints;
        }

        // If there are still points left to allocate...
        if (missingPoints > 0)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
    }

    /// <summary>
    /// Calculates the grades the user should achieve in their assignments, to achieve their grade goals as soon as possible.
    /// </summary>
    private void CalculateSlowestActionableAssignmentGrades()
    {
        // Finds a list of assignments which haven't been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        // Finds a list of assignments which haven't been completed, in reverse order
        List<StudentAssignmentInputViewModel> incompleteAssignmentsReversed = incompleteAssignments.ToArray().Reverse().ToList();
    
        // Finds how many points the user needs to acheive to read their overall grade goal.
        double missingPoints = FindMissingPoints();

        // For each missing assignment, display this assignment as an output with the new score
        foreach (StudentAssignmentInputViewModel assignment in incompleteAssignmentsReversed)
        {
            // Initialise a variable containing as many possible points left can be awarded to the current assignment
            double awardedPoints = Math.Max(Math.Min(missingPoints, 15), 1);

            // Display the current assignment with the current number of missing points
            DisplayAssignment(assignment, Math.Round(awardedPoints, 2));

            // Remove the awarded number of missing points from the missing points pool
            missingPoints -= awardedPoints;
        }

        // Reverse the output assignments back to their usual order
        OutputAssignments = new ObservableCollection<StudentAssignmentInputViewModel>(OutputAssignments.ToArray().Reverse().ToList());

        // If there are still points left to allocate...
        if (missingPoints > 0)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
    }

    private void CalculateAntiExamAssignmentGrades()
    {
        // Finds a list of assignments which havent been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        // Initialise a new list to store the list of grades awarded to assignments, in the order they appear
        List<double> gradeEncodings = new List<double>();
        gradeEncodings.AddRange(Enumerable.Repeat(0.0, incompleteAssignments.Count));

        // Initialse an integer variable to count the number of exams
        int examCount = 0;

        // Finds how many points the user needs to acheive to read their overall grade goal.
        double missingPoints = FindMissingPoints();

        // For each missing assignment that isn't an exam, populate with points
        foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
        {
            // If the assignment is not an exam...
            if (!(assignment.AssessmentType == "EXAM"))
            {
                // Initialise a variable containing as many possible points left can be awarded to the current assignment
                double awardedPoints = Math.Max(Math.Min(missingPoints, 15), 1);

                // Award the current assignment this number of points
                gradeEncodings[incompleteAssignments.IndexOf(assignment)] = awardedPoints;

                // Remove the awarded number of missing points from the missing points pool
                missingPoints -= awardedPoints;
            }
            else
            {
                examCount++;
            }
        }

        // If this course does have an exam...
        if (examCount > 0)
        {
            // Calculate the remaining fraction of grades dedicated to exams
            double remainingGrade = Math.Max(Math.Min(missingPoints / examCount, 15), 1);

            // Then, for each missing assignment that is an exam, populate with the remaining points
            foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
            {
                // If the assignment is an exam...
                if (assignment.AssessmentType == "EXAM")
                {
                    // Award the current exam this number of points
                    gradeEncodings[incompleteAssignments.IndexOf(assignment)] = remainingGrade;


                    // Remove the awarded number of missing points from the missing points pool
                    missingPoints -= remainingGrade;
                }
            }
        }

        // Finally, display the assignments with their respective grades
        foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
        {
            // Load the assignment's grade from its grade encoding
            double grade = gradeEncodings[incompleteAssignments.IndexOf(assignment)];

            // Display the current assignment with the current number of missing points
            DisplayAssignment(assignment, Math.Round(grade, 2));
        }

        // If there are still points left to allocate...
        if (missingPoints > 0)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
    }

    private void CalculateAntiFolioAssignmentGrades()
    {
        // Finds a list of assignments which havent been completed
        List<StudentAssignmentInputViewModel> incompleteAssignments = FindIncompleteAssignments();

        // Initialise a new list to store the list of grades awarded to assignments, in the order they appear
        List<double> gradeEncodings = new List<double>();
        gradeEncodings.AddRange(Enumerable.Repeat(0.0, incompleteAssignments.Count));

        // Initialse an integer variable to count the number of exams
        int examCount = 0;

        // Finds how many points the user needs to acheive to read their overall grade goal.
        double missingPoints = FindMissingPoints();

        // For each missing assignment that isn't an exam, populate with points
        foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
        {
            // If the assignment is not an exam...
            if (!(assignment.AssessmentType == "FOLIO"))
            {
                // Initialise a variable containing as many possible points left can be awarded to the current assignment
                double awardedPoints = Math.Max(Math.Min(missingPoints, 15), 1);

                // Award the current assignment this number of points
                gradeEncodings[incompleteAssignments.IndexOf(assignment)] = awardedPoints;

                // Remove the awarded number of missing points from the missing points pool
                missingPoints -= awardedPoints;
            }
            else
            {
                examCount++;
            }
        }

        // If this course does have an exam...
        if (examCount > 0)
        {
            // Calculate the remaining fraction of grades dedicated to exams
            double remainingGrade = Math.Max(Math.Min(missingPoints / examCount, 15), 1);

            // Then, for each missing assignment that is an exam, populate with the remaining points
            foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
            {
                // If the assignment is an exam...
                if (assignment.AssessmentType == "EXAM")
                {
                    // Award the current exam this number of points
                    gradeEncodings[incompleteAssignments.IndexOf(assignment)] = remainingGrade;


                    // Remove the awarded number of missing points from the missing points pool
                    missingPoints -= remainingGrade;
                }
            }
        }

        // Finally, display the assignments with their respective grades
        foreach (StudentAssignmentInputViewModel assignment in incompleteAssignments)
        {
            // Load the assignment's grade from its grade encoding
            double grade = gradeEncodings[incompleteAssignments.IndexOf(assignment)];

            // Display the current assignment with the current number of missing points
            DisplayAssignment(assignment, Math.Round(grade, 2));
        }

        // If there are still points left to allocate...
        if (missingPoints > 0)
        {
            // Specify that the student's grade goal is unachaivable for this model
            IsGradeGoalAchievable = false;
            IsGradeGoalUnachievable = true;
        }
    }

    #region Calculation Helpers

    private double FindMissingPoints()
    {
        // Initialise a variable to store the grade sum as zero
        double currentGradeSum = 0;

        // For each assignment in all input assignments...
        foreach (StudentAssignmentInputViewModel assignment in InputAssignments)
        {
            currentGradeSum += Convert.ToDouble(assignment.Grade);
        }

        // Using the derived formula x = g - s, where x is the sum of unknwown grades, g is the goal, and s is the sum of known grades
        // Calculate the grade points needed left to acheive the student's goal
        return Convert.ToDouble(GradeGoal) * InputAssignments.Count() - currentGradeSum;
    }

    /// <summary>
    /// Finds the grade needed for all incomplete assignments to achieve the user's pure weight grade goal.
    /// </summary>
    /// <returns>The required grade</returns>
    private double FindPureWeightGradeGoal()
    {
        // Find the number of points missing to reach the user's grade goal
        double missingPoints = FindMissingPoints();

        // Find the average score needed to make up for the user's missing points
        return missingPoints / FindIncompleteAssignments().Count;
    }

    /// <summary>
    /// Finds the grade needed for all incomplete assignments to achieve the user's performance standard grade goal.
    /// </summary>
    /// <returns>The required grade</returns>
    private double FindPerformanceStandardGradeGoal()
    {
        List<StudentAssignmentInputViewModel> assignments = CloneAssignmentInputs(InputAssignments.ToList());
        List<int> incompleteAssignmentIndicides = new List<int>();

        // For every assignment in the list of assignment inputs...
        foreach (StudentAssignmentInputViewModel assignment in assignments)
        {
            // If the assignment has been left empty
            if (string.IsNullOrEmpty(assignment.Grade))
            {
                // Delegate it as such
                incompleteAssignmentIndicides.Add(assignments.IndexOf(assignment));
            }
        }

        // For every grade between 1 and 15, starting at 1 and incrementing by 0.1...
        for (double averageGrade = 1; !(averageGrade > 16); averageGrade += 0.1 )
        {
            // For each incomplete assignments...
            foreach (int i in incompleteAssignmentIndicides)
            {
                // Set the assignment's grade to the current grade
                assignments[i].Grade = averageGrade.ToString();
            }

            // If using this grade allows the user to achaive their desired performance standard grade
            if (FindPerformanceStandardGrade(assignments) > Convert.ToDouble(GradeGoal))
            {
                // Return this grade
                return averageGrade;
            }
        }

        return 15;
    }

    #endregion

    #region Performance Standard Grade Core

    /// <summary>
    /// Displays the user's performance standard-based grade for this course, and their weakest area
    /// </summary>
    private double FindPerformanceStandardGrade(List<StudentAssignmentInputViewModel> assignments)
    {
        // Find the performance standards for this subject's course
        List<string> Standards = DatabaseHelpers.FindSubjectPerformanceStandards(SelectedCourse);

        // Find the grade sums and counts for the performance standards following this course's assessment
        Sums = FindPerformanceStandardGradeSums(Standards, assignments);
        Occurences = FindPerformanceStandardOccurences(Standards, assignments);

        if (Sums.Sum() > 0)
        {
            // Calculate the grades for each performance standard following this course's assessment
            Grades = FindPerformanceStandardGrades(Standards, Sums, Occurences);

            // Calculate both representations for the pure weight grade, excluding performance standards that have no assignments attributed to them
            return Math.Round(Grades.Sum() / Occurences.Where(x => x > 0).Count(), 2);
        }
        else
        {
            // Return 0 [IMPLEMENT ERROR]
            return 0;
        }
    }

    /// <summary>
    /// Finds the total scores for each performance standard in a course.
    /// </summary>
    /// <param name="subjectPerformanceStandards">The course's performance standards</param>
    /// <returns>The total scores for this course's performance standards</returns>
    private List<double> FindPerformanceStandardGradeSums(List<string> subjectPerformanceStandards, List<StudentAssignmentInputViewModel> assignments)
    {
        // Load the assignment, course and subject databases
        List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

        // Initialise two lists of zeroes, equal in length to the number of performance standards), to store a running total for the grade sums for each performance standard, and a running total of each performance standard's occurences
        List<double> performanceStandardGradeSums = new List<double>();
        performanceStandardGradeSums.AddRange(Enumerable.Repeat(0.0, subjectPerformanceStandards.Count));

        // For each assignment in the currently selected course's list of assignments
        foreach (StudentAssignmentInputViewModel assignment in assignments)
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
    private List<double> FindPerformanceStandardOccurences(List<string> subjectPerformanceStandards, List<StudentAssignmentInputViewModel> assignments)
    {
        // Load the assignment, course and subject databases
        List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

        // Initialise two lists of zeroes, equal in length to the number of performance standards), to store a running total for the grade sums for each performance standard, and a running total of each performance standard's occurences
        List<double> performanceStandardGradeCounts = new List<double>();
        performanceStandardGradeCounts.AddRange(Enumerable.Repeat(0.0, subjectPerformanceStandards.Count));

        // For each assignment in the currently selected course's list of assignments
        foreach (StudentAssignmentInputViewModel assignment in assignments)
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

    #endregion

    #region Pop-Up Core

    /// <summary>
    /// The command for when a pop-up is closed
    /// </summary>
    public ICommand ClosePopUpCommand { get; set; }

    /// <summary>
    /// Closes the current pop-up.
    /// </summary>
    private void ClosePopUp()
    {
        PopUpAggregator.BroadcastDeletion();
    }

    #endregion

    #region Legacy

    /// <summary>
    /// Clones the observable collection of assignment inputs while breaking their, unwanted data connection.
    /// </summary>
    /// <param name="oldCollection">The original collection of assignment inputs</param>
    /// <returns>The cloned collection of assignment inputs</returns>
    private List<StudentAssignmentInputViewModel> CloneAssignmentInputs(List<StudentAssignmentInputViewModel> oldCollection)
    {
        List<StudentAssignmentInputViewModel> newCollection = new List<StudentAssignmentInputViewModel>();

        foreach (StudentAssignmentInputViewModel assignment in oldCollection)
        {
            StudentAssignmentInputViewModel addedAssignment = new StudentAssignmentInputViewModel
            {
                AssessmentType = assignment.AssessmentType,
                Course = assignment.Course,
                Name = assignment.Name,
                DisplayName = assignment.DisplayName,
                Icon = assignment.Icon,
                Grade = assignment.Grade,
                Weight = assignment.Weight,
                IsReadOnly = true
            };

            newCollection.Add(addedAssignment);
        }

        return newCollection;
    }

    #endregion
}
