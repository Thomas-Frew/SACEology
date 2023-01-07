namespace SACEology
{
    public enum ApplicationPage
    {
        /// <summary>
        /// The application's Main Menu.
        /// </summary>
        MainMenu = 0,

        /// <summary>
        /// The student portal, with all the student functionality.
        /// </summary>
        StudentPortal = 1,

        /// <summary>
        /// The student's list of courses.
        /// </summary>
        StudentMyCourses = 2,

        /// <summary>
        /// A breakdown of assignments for the currently selected course, from the student's perspective.
        /// </summary>
        StudentCourse = 3,

        /// <summary>
        /// A breakdown of the currently selected assignment, from the student's perspective.
        /// </summary>
        StudentAssignment = 4,

        /// <summary>
        /// The student's dashboard, where they can manage and design plans to enact their studies.
        /// </summary>
        StudentMyDashboard = 5,

        /// <summary>
        /// The student's results hub, where they can predict their grades, plan out their future ahcaivement, and see how they are scaled against students across the state.
        /// </summary>
        StudentMyResults = 6,

        /// <summary>
        /// The teacher portal, with all the reacher functionality.
        /// </summary>
        TeacherPortal = 7,

        /// <summary>
        /// The teacher's list of courses.
        /// </summary>
        TeacherMyCourses = 8,

        /// <summary>
        /// A breakdown of assignments for the currently selected course, from the teacher's perspective.
        /// </summary>
        TeacherCourse = 9,

        /// <summary>
        /// A breakdown of the currently selected assignment, from the teacher's perspective.
        /// </summary>
        TeacherAssignment = 10,

        /// <summary>
        /// A menu where teachers can access SACE resources from a dynamically created menu
        /// </summary>
        TeacherMyCommunity = 11,
    }
}
