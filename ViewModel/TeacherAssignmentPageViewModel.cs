using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SACEology.ViewModel
{
    class TeacherAssignmentPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Whether changes in this assignment's properties should be written to the database.
        /// </summary>
        public bool Editable { get; set; } = false;

        /// <summary>
        /// The assignment's subject code.
        /// </summary>
        public string SubjectCode { get; set; }

        /// <summary>
        /// The assignment's parent course.
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// The assignment's assessment type.
        /// </summary>
        public string AssessmentType { get; set; } = "ERR";

        /// <summary>
        ///  The assignment's name.
        /// </summary>
        public string Name { get; set; } = Settings.Default.SelectedAssignment;

        /// <summary>
        /// The assignment's icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// A private variable storing the assignment's description.
        /// </summary>
        private string _description { get; set; }

        /// <summary>
        /// The assignment's description.
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                SaveAssignmentData();
            }
        }

        /// <summary>
        /// A private variable storing the assignment's starting date.
        /// </summary>
        private string _startingDate { get; set; }

        /// <summary>
        /// The assignment's starting date.
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
                SaveAssignmentData();
            }
        }

        /// <summary>
        /// A private variable storing the assignment's due date.
        /// </summary>
        private string _dueDate { get; set; }

        /// <summary>
        /// The assignment's due date.
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
                SaveAssignmentData();
            }
        }

        /// <summary>
        /// The assignment's weight.
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// The assignment's performance standards, packaged as a dense string.
        /// </summary>
        public string PackagedPerformanceStandards { get; set; }

        /// <summary>
        /// The assignment's perfomrnace standards, unpackaged into a collection of view models
        /// </summary>
        public ObservableCollection<AspectBadgeViewModel> PerformanceStandards { get; set; } = new ObservableCollection<AspectBadgeViewModel>();

        /// <summary>
        /// The number of new messages this assignment has
        /// </summary>
        public string Messages { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the new messages button is pressed, to view students' messages.
        /// </summary>
        public ICommand ShowMessagesPopUpCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public TeacherAssignmentPageViewModel()
        {
            LoadAssignmentProperties();
            DisplayPerformanceStandards();

            // Initialise button relay commands
            ShowMessagesPopUpCommand = new RelayCommand(() => ShowMessagesPopUp());
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Load the assignment's properties from the assignment database.
        /// </summary>
        private void LoadAssignmentProperties()
        {
            // Load a database of all assignments
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Load a database of all messages
            List<List<string>> messageDatabase = DatabaseHelpers.LoadAssignmentMessageDatabase();

            // Unpack this assignment's properties
            foreach (List<string> assignment in assignmentDatabase)
            {
                if (Name == assignment[(int)AProp.Name])
                {
                    SubjectCode = assignment[(int)AProp.SubjectCode];
                    Course = assignment[(int)AProp.Course];
                    AssessmentType = assignment[(int)AProp.AssessmentType];
                    Icon = DatabaseHelpers.FindAssignmentIcon(assignment);
                    Description = assignment[(int)AProp.Description];
                    StartingDate = assignment[(int)AProp.StartingDate];
                    DueDate = assignment[(int)AProp.DueDate];
                    Weight = assignment[(int)AProp.Weight];
                    PackagedPerformanceStandards = assignment[(int)AProp.PerformanceStandards];
                }
            }

            // For every message in the message database which is connected to this assignment, add one to the message count
            int messages = new int();
            foreach (List<string> message in messageDatabase)
            {
                if (Name == message[(int)AMProp.Assignment])
                {
                    messages++;
                }
            }
            Messages = messages.ToString();

            // Make the assignment open to editing
            Editable = true;
        }

        /// <summary>
        /// Unpackage the assignment's perfomrnace standards and display them
        /// </summary>
        private void DisplayPerformanceStandards()
        {
            // Initialise a list of all this assignment's performance standards
            List<string> performanceStandards = PackagedPerformanceStandards.Split((string[])null, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Display the assignment's performance standards
            foreach (string standard in performanceStandards)
            {
                AspectBadgeViewModel addedStandard = new AspectBadgeViewModel { Code = standard, RootCode = standard.Remove(standard.Length - 1) };
                PerformanceStandards.Add(addedStandard);
            }
        }

        /// <summary>
        /// Saves changes made to the assignment for the current assignment to assignment database.
        /// </summary>
        private void SaveAssignmentData()
        {
            // If the assignment has been loaded...
            if (Editable)
            {
                // Load the master database of assignments
                List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

                // For every index of every assignment in the database...
                foreach (int i in Enumerable.Range(0, assignmentDatabase.Count))
                {
                    // If the index of this assigment is found... 
                    if (Name == assignmentDatabase[i][(int)AProp.Name])
                    {
                        // Replace the assignment in the assignment database with the new data
                        List<string> assignmentData = new List<string> { SubjectCode, Course,
                                                             AssessmentType, Name, Icon, Description,
                                                             StartingDate, DueDate, Weight, PackagedPerformanceStandards, Messages };
                        assignmentDatabase[i] = assignmentData;
                    }
                }

                // Save the assignment database, with the replaced data
                DatabaseHelpers.SaveAssignmentDatabase(assignmentDatabase);
            }
        }

        #endregion

        #region Pop-Up Core

        /// <summary>
        /// Shows the "Messages" pop up.
        /// </summary>
        private void ShowMessagesPopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastMessagesPopUpCreation();
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
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.TeacherCourse);

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
