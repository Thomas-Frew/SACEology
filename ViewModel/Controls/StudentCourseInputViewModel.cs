using System;
using SACEology.ViewModel.Base;
using static SACEology.ValidationHelpers;

namespace SACEology.ViewModel
{
    class StudentCourseInputViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The course's subject code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The course's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The course's name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The course's FontAwesome icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The number of new messages for this course
        /// </summary>
        public string NewMessages { get; set; }

        /// <summary>
        /// The course's subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The course's subject's scaling score.
        /// </summary>
        public string ScalingScore { get; set; }

        /// <summary>
        /// A private variable storing the grade achieved for the course.
        /// </summary>
        private string _grade { get; set; }

        /// <summary>
        /// The grade acheived for the course.
        /// </summary>
        public string Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                _grade = ValidateGrade(value);

                // Broadcast that a grade has been edited
                ValidationAggregator.BroadcastGradeEditing();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public StudentCourseInputViewModel() { }

        #endregion
    }
}
