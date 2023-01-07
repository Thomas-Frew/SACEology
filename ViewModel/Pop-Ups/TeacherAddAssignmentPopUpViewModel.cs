using SACEology.ViewModel.Base;
using System.Windows.Input;
using System.Linq;
using SACEology;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SACEology.ViewModel;
using System;
using SACEology.Properties;

class TeacherAddAssignmentPopUpViewModel : BaseViewModel
{
    #region Public Properties

    public string Course { get; set; } = Settings.Default.SelectedCourse;

    /// <summary>
    /// The parent course's currently selected subject.
    /// </summary>
    public string Subject { get; set; } = Settings.Default.SelectedSubject;

    /// <summary>
    /// The assignment's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The assignment's description.
    /// </summary>
    public string Description { get; set; }

    #region Dates

    /// <summary>
    /// A private variable storing when the assignment begins.
    /// </summary>
    private string _startingDate;

    /// <summary>
    /// When the assignment begins.
    /// </summary>
    public string StartingDate
    {
        get
        {
            return _startingDate;
        }
        set
        {
            _startingDate = ValidationHelpers.ValidateStartingDate(value, DueDate);
        }
    }

    /// <summary>
    /// A private variable storing when the assignment is due.
    /// </summary>
    private string _dueDate;

    /// <summary>
    /// When the assignment is due.
    /// </summary>
    public string DueDate
    {
        get
        {
            return _dueDate;
        }
        set
        {
            _dueDate = ValidationHelpers.ValidateDueDate(StartingDate, value);
        }
    }

    #endregion

    /// <summary>
    /// A private variable storing the assignment's weight in its course.
    /// </summary>
    public string _weight { get; set; }

    /// <summary>
    /// The assignment's weight in its course.
    /// </summary>
    public string Weight
    {
        get
        {
            return _weight;
        }
        set
        {
            _weight = ValidationHelpers.ValidateWeight(Course, value);
        }
    }

    /// <summary>
    /// A list of the subject's current performance standards.
    /// </summary>
    public ObservableCollection<AspectBadgeViewModel> PerformanceStandards { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

    /// <summary>
    /// A list of the subject's current assessment types.
    /// </summary>
    public ObservableCollection<AspectBadgeViewModel> AssessmentTypes { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

    public int SelectedAssessmentIndex { get; set; } = 0;

    #endregion

    #region Public Commands

    /// <summary>
    /// The command for when the "Create Assignment" button is pressed, to create a course.
    /// </summary>
    public ICommand CreateAssignmentCommand { get; set; }

    /// <summary>
    /// The command for when the plus button is pressed, to decrement and assignment's weighting.
    /// </summary>
    public ICommand DecrementWeightCommand { get; set; }

    /// <summary>
    /// The command for when the plus button is pressed, to increment and assignment's weighting.
    /// </summary>
    public ICommand IncrementWeightCommand { get; set; }

    #endregion 

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public TeacherAddAssignmentPopUpViewModel()
    {
        RefreshSubjectProperties();

        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());
        CreateAssignmentCommand = new RelayCommand(() => ValidateAssignmentCreation());
        DecrementWeightCommand = new RelayCommand(() => DecrementWeight());
        IncrementWeightCommand = new RelayCommand(() => IncrementWeight());
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Loads the subject performance standards and assessment types.
    /// </summary>
    public void RefreshSubjectProperties()
    {
        // Clear the currently selected performance standards and assessment types...
        PerformanceStandards.Clear();
        AssessmentTypes.Clear();

        // Load the subjects database
        List<List<string>> subjects = DatabaseHelpers.LoadSubjectDatabase();

        // For each subject in the list of subject...
        foreach (List<string> subject in subjects)
        {
            // If the subject entry is the course's selected subject...
            if (subject[(int)SProp.Name] == Subject)
            {
                // Get a list of performance standards for the current subject
                List<string> performanceStandards = subject[(int)SProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> assessmentTypes = subject[(int)SProp.AssessmentTypes].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                // For each performance standard, add to performance standard to the display
                foreach (string standard in performanceStandards)
                {
                    AspectBadgeViewModel badge = new AspectBadgeViewModel() { Code = standard, RootCode = standard.Remove(standard.Length - 1) };
                    PerformanceStandards.Add(badge);
                }

                // For each performance standard, add to performance standard to the display
                foreach (string assessment in assessmentTypes)
                {
                    AspectBadgeViewModel badge = new AspectBadgeViewModel() { Code = assessment, RootCode = assessment };
                    AssessmentTypes.Add(badge);
                }
            }
        }
    }

    /// <summary>
    /// Creates an assignment only when all the nessecary conditions are satisfied, otherwise displaying an error message.
    /// </summary>
    private void ValidateAssignmentCreation()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            PopUpAggregator.BroadcastErrorPopUpCreation("Please enter this assignment's name.");
        }
        else if (string.IsNullOrWhiteSpace(Description))
        {
            PopUpAggregator.BroadcastErrorPopUpCreation("Please enter a description for this assignment.");
        }
        else if (string.IsNullOrWhiteSpace(StartingDate) || string.IsNullOrWhiteSpace(DueDate))
        {
            PopUpAggregator.BroadcastErrorPopUpCreation("Please enter a starting and due date for this assignment.");
        }
        else if (string.IsNullOrWhiteSpace(StartingDate) || string.IsNullOrWhiteSpace(DueDate))
        {
            PopUpAggregator.BroadcastErrorPopUpCreation("Please enter a weight for this assignment.");
        }
        else
        {
            CreateAssignment();
        }
    }

    /// <summary>
    /// Adds the current course to the database of courses
    /// </summary>
    private void CreateAssignment()
    {
        // Locate the subject code for this course
        string subjectCode = string.Empty;
        List<List<string>> subjects = DatabaseHelpers.LoadSubjectDatabase();
        foreach (List<string> subject in subjects)
        {
            if (subject[(int)SProp.Name] == Subject)
            {
                subjectCode = subject[(int)SProp.SubjectCode];
            } 
        }

        // Find the selected performance standards for this assignment
        string performanceStandards = string.Empty;
        foreach (AspectBadgeViewModel standard in PerformanceStandards)
        {
            if (standard.IsSelected)
            {
                performanceStandards += standard.Code + " ";
            }
        }

        string asessmentType = string.Empty;
        foreach (AspectBadgeViewModel standard in AssessmentTypes)
        {
            if (standard.IsSelected)
            {
                asessmentType = standard.Code;
            }
        }

        // Add this assignment to the list of assignments
        List<string> assignment = new List<string>
        {
            subjectCode, Course,
            asessmentType, Name,
            Description, StartingDate, DueDate, Weight, performanceStandards,     
        };

        // Add the assignment to the assignment database
        DatabaseHelpers.AppendAssignmentToDatabase(assignment);

        // Broadcast that an assignment has been created and close the pop up
        DatabaseAggregator.BroadcastAssignmentCreation();
        ClosePopUp();

        // Confirm that the assignment has been created
        PopUpAggregator.BroadcastConfirmationPopUpCreation("Assignment successfully created");
    }

    /// <summary>
    /// Decreases the current assignment's weight by 1%.
    /// </summary>
    private void DecrementWeight()
    {
        int weight = Convert.ToInt32(Weight);

        if (weight - 5 < 0)
        {
            Weight = "0";
        }
        else
        {
            Weight = Convert.ToString(weight - 1);
        }
    }

    /// <summary>
    /// Increases the assignment's weight by 1%.
    /// </summary>
    private void IncrementWeight()
    {
        // Initialise variables for the assignement's current weight and sum of weight for this course
        int weight = Convert.ToInt32(Weight);
        int weightSum = new int();

        // Calculate the sum of weight for this course, and therefore the maximum weight that can be given to this assignment
        List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

        foreach (List<string> assessment in assignmentDatabase)
        {
            if (assessment[(int)AProp.Course] == Course)
            {
                weightSum += Convert.ToInt32(assessment[(int)AProp.Weight]);
            }
        }
        int weightLimit = 100 - weightSum;

        if (weight + 5 > weightLimit)
        {
            Weight = Convert.ToString(weightLimit);
        }
        else
        {
            Weight = Convert.ToString(weight + 1);
        }
    }

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
}
