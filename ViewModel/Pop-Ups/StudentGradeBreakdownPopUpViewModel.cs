using SACEology.ViewModel.Base;
using System.Windows.Input;
using SACEology;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using SACEology.ViewModel;
using SACEology.Properties;

class StudentGradeBreakdownPopUpViewModel : BaseViewModel
{
    #region Public Properties

    /// <summary>
    /// The user's selected course
    /// </summary>
    public string Course = Settings.Default.SelectedCourse;

    /// <summary>
    /// A collection of performance standard breakdowns
    /// </summary>
    public ObservableCollection<StudentPerformanceStandardBreakdownViewModel> Breakdowns { get; set; } = new ObservableCollection<StudentPerformanceStandardBreakdownViewModel>();

    /// <summary>
    /// A list of this course's performance standards.
    /// </summary>
    public List<string> Standards { get; set; }

    /// <summary>
    /// A list encoding of occurences of performance standards within this course.
    /// </summary>
    public List<double> Occurences { get; set; }

    /// <summary>
    /// A list encoding of grades achieved for performance standards within this course.
    /// </summary>
    public List<double> Grades { get; set; }

    public string DisplayText { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public StudentGradeBreakdownPopUpViewModel(List<string> standards, List<double> occurences, List<double> grades)
    {
        Standards = standards;
        Occurences = occurences;
        Grades = grades;

        GenerateDisplayText();
        DisplayPerformanceStandards();

        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());
    }

    /// <summary>
    /// Generates the dynamic, introductory text based on the parent course and grade achieved
    /// </summary>
    private void GenerateDisplayText()
    {
        // Caclualte the user's overall numeric grade and use this to create a display grade
        double numericGrade = Math.Round(Grades.Sum() / Occurences.Where(x => x > 0).Count(), 2);
        string displayGrade = ValidationHelpers.NumericToDisplayGrade(numericGrade);

        DisplayText = "Your " + displayGrade + " grade in " + Course + " is split among " + Standards.Count.ToString() + " performance standards.";
    }

    /// <summary>
    /// Displays the course's performance standards, their occurences, and their grades
    /// </summary>
    private void DisplayPerformanceStandards()
    {
        // For each integer between 0 and the number of performance standards
        foreach (int i in Enumerable.Range(0, Standards.Count))
        {
            // Initialise variables for this standard's code, number of occurences and numeric grade
            string code = Standards[i];
            string occurence = Occurences[i].ToString();

            // Create an aspect badge from this performance standard's code
            AspectBadgeViewModel badge = new AspectBadgeViewModel { Code = code, RootCode = code.Remove(code.Length - 1) };

            // Initialise a variable to store this perofmrance standards name
            string name = string.Empty;

            // Load the performance standard database
            List<List<string>> performanceStandardDatabase = DatabaseHelpers.LoadperformanceStandardDatabase();

            // For each performance standard in the database...
            foreach (List<string> standard in performanceStandardDatabase)
            {
                // If the performance standard's root code matches the current performance standard's root code
                if (standard[(int)PSProp.Code] == code.Remove(code.Length - 1))
                {
                    // Use this code to determine the performance standard's name
                    name = standard[(int)PSProp.Name];
                }
            }

            string displayGrade = ValidationHelpers.NumericToDisplayGrade(Math.Round(Grades[i], 2));

            // Create a new control to display this performance standard
            StudentPerformanceStandardBreakdownViewModel addedBreakdown = new StudentPerformanceStandardBreakdownViewModel
            {
                PerformanceStandardBadge = badge,
                Name = name,
                Occurences = occurence,
                Grade = displayGrade
            };

            // Add this control to the display
            Breakdowns.Add(addedBreakdown);
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
