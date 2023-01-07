namespace SACEology
{
    public enum AMProp
    {
        /// <summary>
        /// The status of the message's sender (teacher or student).
        /// </summary>
        Status = 0,

        /// <summary>
        /// The message's associated course.
        /// </summary>
        Course = 1,

        /// <summary>
        /// The message's assocated assignment.
        /// </summary>
        Assignment = 2,

        /// <summary>
        /// The message's sender's name.
        /// </summary>
        Name = 3,

        /// <summary>
        /// The message's content.
        /// </summary>
        Content = 4,

        /// <summary>
        /// The message's sending date.
        /// </summary>
        Date = 5,

        /// <summary>
        /// The message's sending time.
        /// </summary>
        Time = 6,

        /// <summary>
        /// The message's audio's directory.
        /// </summary>
        Audio = 7,

        /// <summary>
        /// The message's attachment's directory.
        /// </summary>
        Attachment = 8
    }
}
