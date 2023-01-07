namespace SACEology
{
    public enum GoalSeekerCalculation
    {
        /// <summary>
        /// A calculator to get the user to most efficiently achaive their goals.
        /// </summary>
        MostEfficient = 0,

        /// <summary>
        /// A calculator to get the user to acheive their goals with 100% certainty.
        /// </summary>
        GuaranteedEntry = 1,

        /// <summary>
        /// A calculator to get the user to achieve their goals as quickly as possible.
        /// </summary>
        FastestActionable = 2,

        /// <summary>
        /// A calculator to get the user to achieve their goals as lately as possible.
        /// </summary>
        SlowestActionable = 3,

        /// <summary>
        /// A calculator to get the user to achieve their goals, while putting as little stock into exams as possible.
        /// </summary>
        AntiExam = 4,

        /// <summary>
        /// A calculator to get the user to achieve their goals, while putting as little stock into folios as possible.
        /// </summary>
        AntiFolio = 5
    }
}
