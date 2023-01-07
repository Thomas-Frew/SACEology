using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SACEology.ViewModel
{
    class StudentAssignmentPageViewModel : BaseViewModel
    {
        #region Public Properties

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
        /// The assignment's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The assignment's starting date.
        /// </summary>
        public string StartingDate { get; set; }

        /// <summary>
        /// The assignment's due date.
        /// </summary>
        public string DueDate { get; set; }

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
        /// The command for when the new messages button is pressed, to send a new message.
        /// </summary>
        public ICommand ShowSendMessagePopUpCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentAssignmentPageViewModel()
        {
            LoadAssignmentProperties();
            DisplayPerformanceStandards();

            // Initialise button relay commands
            ShowSendMessagePopUpCommand = new RelayCommand(() => ShowSendMessagePopUp());
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
        /// View the students' messages for this assignment [IMPLEMENT]
        /// </summary>
        private void ShowSendMessagePopUp()
        {
            PopUpAggregator.BroadcastMessagesPopUpCreation();
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
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.StudentCourse);

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
