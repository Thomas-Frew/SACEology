using SACEology.ViewModel.Base;
using System.Windows.Input;
using System.Linq;
using SACEology;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SACEology.ViewModel;
using System;
using System.Diagnostics;

class TeacherAddCoursePopUpViewModel : BaseViewModel
{
    #region Public Properties

    /// <summary>
    /// A list of all subject names.
    /// </summary>
    public List<string> Subjects { get; set; } = DatabaseHelpers.LoadSubjectDatabase().Select(array => array[(int)SProp.Name]).ToList();

    /// <summary>
    /// A private variable storing the course's subject.
    /// </summary>
    private string _subject { get; set; } = "Accounting";

    /// <summary>
    /// The course's subject.
    /// </summary>
    public string Subject
    {
        get { return _subject; }
        set
        {
            _subject = value;
            RefreshSubjectProperties();
        }
    }

    /// <summary>
    /// The course's name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A list of the subject's current performance standards.
    /// </summary>
    public ObservableCollection<AspectBadgeViewModel> PerformanceStandards { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

    /// <summary>
    /// A list of the subject's current assessment types.
    /// </summary>
    public ObservableCollection<AspectBadgeViewModel> AssessmentTypes { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

    #endregion

    #region Public Commands

    /// <summary>
    /// The command for when the atlas button is pressed, to open the SACE overview of the currently selected subject
    /// </summary>
    public ICommand OpenSubjectOverviewCommand { get; set; }

    /// <summary>
    /// The command for when the "Create Course" button is pressed, to create a course.
    /// </summary>
    public ICommand CreateCourseCommand { get; set; }

    #endregion 

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public TeacherAddCoursePopUpViewModel()
    {
        // Load the inital subject's performance standards and assessment types
        RefreshSubjectProperties();

        // Listen out for commands called by the view
        OpenSubjectOverviewCommand = new RelayCommand(() => OpenSubjectOverview());
        CreateCourseCommand = new RelayCommand(() => ValidateCourseCreation());
        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());

    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Refreshes the subject performance standards and assessment types when a new subject is selected.
    /// </summary>
    public void RefreshSubjectProperties()
    {
        List<List<string>> subjects = DatabaseHelpers.LoadSubjectDatabase();

        // Clear the currently selected performance standards and assessment types...
        PerformanceStandards.Clear();
        AssessmentTypes.Clear();

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
    /// Creates an course only when all the nessecary conditions are satisfied, otherwise displaying an error message.
    /// </summary>
    public void ValidateCourseCreation()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            PopUpAggregator.BroadcastErrorPopUpCreation("Please enter this course's name.");
        }
        else
        {
            CreateCourse();
        }
    }

    /// <summary>
    /// Adds the current course to the database of courses
    /// </summary>
    private void CreateCourse()
    {
        // Subject code defaults to an unknown, and scaling score to 0 if no subject match are found. Theoretically impossible, but useful to have.
        string subjectCode = "UNK";
        string scalingScore = "0";
        List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();

        // For each subject in the list ob subjects...
        foreach (List<string> subject in subjectDatabase)
        {
            // If this course's subject is the same as the current subject...
            if (subject[(int)SProp.Name] == Subject)
            {
                // Set the course's subject code and scaling score to the current subject code and scaling score
                subjectCode = subject[(int)SProp.SubjectCode];
                scalingScore = subject[(int)SProp.ScalingScore];
            } 
        }

        // Create a course with all the nessecary course data
        List<string> addedCourse = new List<string> { subjectCode, Name, scalingScore };

        // Add the message to the message database
        DatabaseHelpers.AppendCourseToDatabase(addedCourse);

        // Broadcast that a course has been created
        DatabaseAggregator.BroadcastCourseCreation();

        // Close the pop up
        ClosePopUp();

        // Confirm that the course has been created
        PopUpAggregator.BroadcastConfirmationPopUpCreation("Course successfully created.");
    }

    /// <summary>
    /// Opens the SACE overview of the currently selected subject in Chrome.
    /// </summary>
    private void OpenSubjectOverview()
    {
        // Find what the page of the currently selected subject would look like online, which uses hypens instead of spaces and is all lower-case.
        string subjectPage = Subject.Replace(" ", "-").ToLower();

        // Determine the link of this subject on the SACE website.
        string link = "https://www.sace.sa.edu.au/web/" + subjectPage + "/overview";

        // Open a new chrome tab with the subject's link.
        Process.Start("chrome.exe", link);
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
