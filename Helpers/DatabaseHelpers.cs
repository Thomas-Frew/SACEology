using SACEology.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SACEology
{
    public class DatabaseHelpers
    {
        #region Comma Encryption and Decryption

        /// <summary>
        /// Encrypts a string to a csv-safe format.
        /// </summary>
        /// <param name="rawString">The raw string to encrypt</param>
        /// <returns></returns>
        public static string PackageString(string rawString)
        {
            StringBuilder builder = new StringBuilder(rawString);
            builder.Replace(",", "[COMMA]");
            return builder.ToString();
        }

        /// <summary>
        /// Decrypts a csv-safe string into its original version.
        /// </summary>
        /// <param name="rawString">The raw string to decrypt</param>
        /// <returns></returns>
        public static string UnpackageString(string rawString)
        {
            StringBuilder builder = new StringBuilder(rawString);
            builder.Replace("[COMMA]", ",");
            return builder.ToString();
        }

        #endregion

        #region Database Loading

        /// <summary>
        /// Loads the database of courses from local storage.
        /// </summary>
        /// <returns>The course database.</returns>
        public static List<List<string>> LoadCourseDatabase()
        {
            // Load the database information from CourseDatabase.csv as a list of lists of strings, by splitting each line by commas
            List<List<string>> rawDatabase = File.ReadAllLines("CourseDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            // For each list in rawDatabase, representing a course
            foreach (List<string> course in rawDatabase)
            {
                // If the course name contains the safe-string [COMMA], replace the course's name with a name unpackaged (converting [COMMA]s to actual commas)
                if (course[(int)CProp.Name].Contains("[COMMA]")) { course[(int)CProp.Name] = UnpackageString(course[(int)CProp.Name]); }
            }

            // Filter and return the list of courses to only those which have three properties loaded (the correct number)
            return rawDatabase.Where(c => c.Count == 3).ToList();
        }

        /// <summary>
        /// Loads the database of assignments from local storage.
        /// </summary>
        /// <returns>The assignment database.</returns>
        public static List<List<string>> LoadAssignmentDatabase()
        {
            List<List<string>> rawDatabase = File.ReadAllLines("AssignmentDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            foreach (List<string> assignment in rawDatabase)
            {
                foreach (int x in Enumerable.Range(0, assignment.Count - 1))
                {
                    if (assignment[x].Contains("[COMMA]")) { assignment[x] = UnpackageString(assignment[x]); }
                }
            }

            return rawDatabase.Where(c => c.Count == 9).ToList();

        }

        /// <summary>
        /// Loads the database of subject from local storage.
        /// </summary>
        /// <returns>The course database.</returns>
        public static List<List<string>> LoadSubjectDatabase()
        {
            List<List<string>> rawDatabase = File.ReadAllLines("SubjectDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            return rawDatabase.Where(c => c.Count == 6).ToList();
        }

        /// <summary>
        /// Loads the database of assessment types from local storage.
        /// </summary>
        /// <returns>The course database.</returns>
        public static List<List<string>> LoadAssessmentTypeDatabase()
        {
            List<List<string>> rawDatabase = File.ReadAllLines("AssessmentTypeDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            foreach (List<string> type in rawDatabase)
            {
                foreach (int x in Enumerable.Range(0, type.Count))
                {
                    if (type[x].Contains("[COMMA]")) { type[x] = UnpackageString(type[x]); }
                }
            }

            return rawDatabase.Where(c => c.Count == 4).ToList();
        }

        /// <summary>
        /// Loads the database of assessment types from local storage.
        /// </summary>
        /// <returns>The course database.</returns>
        public static List<List<string>> LoadperformanceStandardDatabase()
        {
            List<List<string>> rawDatabase = File.ReadAllLines("performanceStandardsDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            foreach (List<string> standard in rawDatabase)
            {
                foreach (int x in Enumerable.Range(0, standard.Count))
                {
                    if (standard[x].Contains("[COMMA]")) { standard[x] = UnpackageString(standard[x]); }
                }
            }

            return rawDatabase.Where(c => c.Count == 3).ToList();
        }

        /// <summary>
        /// Loads the database of messages from local storage.
        /// </summary>
        /// <returns>The message database.</returns>
        public static List<List<string>> LoadAssignmentMessageDatabase()
        {
            // Load the assignment message database
            List<List<string>> messageDatabase = File.ReadAllLines("AssignmentMessageDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            // If there are more than zero messages...
            if (messageDatabase.Count > 0)
            {
                // For each message in the message database...
                foreach (List<string> message in messageDatabase)
                {
                    // For each property in the message...
                    foreach (int x in Enumerable.Range(0, message.Count - 1))
                    {
                        // Replace all commas in the property with actual commas
                        if (message[x].Contains("[COMMA]")) { message[x] = UnpackageString(message[x]); }
                    }
                }
            }

            // Filter the message database by messages which exist (not blank lines)
            List<List<string>> filteredMessageDatabase = messageDatabase.Where(c => c.Count == 9).ToList();

            // If messages should be auto-deleted, dispose of all messages older than 10 days
            if (Settings.Default.AutoDeleteMessages)
            {
                // If there are more than zero messages...
                if (filteredMessageDatabase.Count > 0)
                {
                    // Remove all messages older than ten days
                    filteredMessageDatabase = messageDatabase.Where(x => DateTime.Parse(x[(int)AMProp.Date]) > DateTime.Now.AddDays(-10)).ToList();
                }
            }

            return filteredMessageDatabase;
        }

        /// <summary>
        /// Loads the database of messages from local storage.
        /// </summary>
        /// <returns>The message database.</returns>
        public static List<List<string>> LoadCommunityMessageDatabase()
        {
            // Load the assignment message database
            List<List<string>> messageDatabase = File.ReadAllLines("CommunityMessageDatabase.csv").Select(line => line.Split(',').ToList()).ToList();

            if (messageDatabase.Count > 0)
            {
                foreach (List<string> message in messageDatabase)
                {
                    foreach (int x in Enumerable.Range(0, message.Count - 1))
                    {
                        if (message[x].Contains("[COMMA]")) { message[x] = UnpackageString(message[x]); }
                    }
                }
            }

            // If messages should be auto-deleted, dispose of all messages odler than 10 days
            if (Settings.Default.AutoDeleteMessages)
            {
                messageDatabase = messageDatabase.Where(x => DateTime.Parse(x[(int)CMProp.Date]) > DateTime.Now.AddDays(-10)).ToList();
            }

            return messageDatabase.Where(c => c.Count == 7).ToList();
        }

        #endregion

        #region Database Appending

        /// <summary>
        /// Appends a new course onto the end of the course database.
        /// </summary>
        /// <param name="course">The course to be appended</param>
        public static void AppendCourseToDatabase(List<string> course)
        {
            // Load the course database
            List<List<string>> courseDatabase = LoadCourseDatabase();

            // Append the new line to the course database
            courseDatabase.Add(course);

            // Replaced the saved course database with this new context
            SaveCourseDatabase(courseDatabase);
        }

        /// <summary>
        /// Appends a new assignment onto the end of the assignment database.
        /// </summary>
        /// <param name="assignment">The assignment to be appended</param>
        public static void AppendAssignmentToDatabase(List<string> assignment)
        {
            List<List<string>> assignmentDatabase = LoadAssignmentDatabase();
            assignmentDatabase.Add(assignment);
            SaveAssignmentDatabase(assignmentDatabase);
        }

        /// <summary>
        /// Appends a new message onto the end of the message database.
        /// </summary>
        /// <param name="message">The message to be appended</param>
        public static void AppendAssignmentMessageToDatabase(List<string> message)
        {
            List<List<string>> messageDatabase = LoadAssignmentMessageDatabase();
            messageDatabase.Add(message);
            SaveAssignmentMessageDatabase(messageDatabase);
        }

        /// <summary>
        /// Appends a new message onto the end of the message database.
        /// </summary>
        /// <param name="message">The message to be appended</param>
        public static void AppendCommunityMessageToDatabase(List<string> message)
        {
            List<List<string>> messageDatabase = LoadCommunityMessageDatabase();
            messageDatabase.Add(message);
            SaveCommunityMessageDatabase(messageDatabase);
        }

        #endregion

        #region Database Saving

        /// <summary>
        /// Overwrites the course database in local files with a new instance.
        /// </summary>
        /// <param name="courseDatabase"></param>
        public static void SaveCourseDatabase(List<List<string>> courseDatabase)
        {
            // For every property of every course in the database to be saved...
            foreach (int i in Enumerable.Range(0, courseDatabase.Count))
            {
                // Package the property's string (replacing commas with [COMMA] strings)
                courseDatabase[i] = courseDatabase[i].ToList().ConvertAll(s => PackageString(s)).ToList();
            }

            // Sort the course database by subject code (in subject alphabetical order)
            courseDatabase = courseDatabase.OrderBy(x => x[(int)CProp.SubjectCode]).ToList();

            // Initialise an empty list of strings to load the courses into
            List<string> courseDatabaseContent = new List<string>();

            // For each course in the course database...
            foreach (List<string> course in courseDatabase)
            {
                // Replace every list in the list with a comma-seperated by a string
                courseDatabaseContent.Add(string.Join(",", course));
            }

            // Replace the course database content with the new list of strings, courseDatabaseContent
            File.WriteAllLines("CourseDatabase.csv", courseDatabaseContent);
        }

        /// <summary>
        /// Overwrites the assignment database in local files with a new instance.
        /// </summary>
        /// <param name="assignmentDatabase"></param>
        public static void SaveAssignmentDatabase(List<List<string>> assignmentDatabase)
        {
            // For every index of every assignment in the database...
            foreach (int i in Enumerable.Range(0, assignmentDatabase.Count))
            {
                assignmentDatabase[i] = assignmentDatabase[i].ToList().ConvertAll(s => PackageString(s)).ToList();
            }

            // Sort the assignment database
            assignmentDatabase = assignmentDatabase.OrderBy(x => DateTime.Parse(x[(int)AProp.DueDate]).Ticks).ToList();

            List<string> assignmentDatabaseContent = new List<string>();

            foreach (List<string> assignment in assignmentDatabase)
            {
                assignmentDatabaseContent.Add(string.Join(",", assignment));
            }

            File.WriteAllLines("AssignmentDatabase.csv", assignmentDatabaseContent);
        }

        /// <summary>
        /// Overwrites the message database in local files with a new instance.
        /// </summary>
        /// <param name="messageDatabase"></param>
        public static void SaveAssignmentMessageDatabase(List<List<string>> messageDatabase)
        {
            // For every index of every assignment in the database...
            foreach (int i in Enumerable.Range(0, messageDatabase.Count))
            {
                messageDatabase[i] = messageDatabase[i].ToList().ConvertAll(s => PackageString(s)).ToList();
            }

            // Sort the messages displayed by urgency, then title
            messageDatabase = messageDatabase.OrderBy(x => x[(int)AMProp.Time]).ToList();
            messageDatabase = messageDatabase.OrderBy(x => DateTime.Parse(x[(int)AMProp.Date])).ToList();

            // Initialise a new, empty list of strings for the content of the message database csv
            List<string> messageDatabaseContent = new List<string>();

            // For every message in the message database...
            foreach (List<string> message in messageDatabase)
            {
                messageDatabaseContent.Add(string.Join(",", message));
            }

            // Save the message database to local storage
            File.WriteAllLines("AssignmentMessageDatabase.csv", messageDatabaseContent);
        }

        /// <summary>
        /// Overwrites the community message database in local files with a new instance.
        /// </summary>
        /// <param name="communityMessageDatabase">The community message database</param>
        public static void SaveCommunityMessageDatabase(List<List<string>> communityMessageDatabase)
        {
            // For every index of every assignment in the database...
            foreach (int i in Enumerable.Range(0, communityMessageDatabase.Count))
            {
                communityMessageDatabase[i] = communityMessageDatabase[i].ToList().ConvertAll(s => PackageString(s)).ToList();
            }

            // Sort the community messages displayed by urgency, then title
            communityMessageDatabase = communityMessageDatabase.OrderBy(x => x[(int)CMProp.Time]).ToList();
            communityMessageDatabase = communityMessageDatabase.OrderBy(x => DateTime.Parse(x[(int)CMProp.Date])).ToList();

            // Initialise a new, empty list of strings for the content of the message database csv
            List<string> communityMessageDatabaseContent = new List<string>();

            // For every message in the community message database...
            foreach (List<string> message in communityMessageDatabase)
            {
                communityMessageDatabaseContent.Add(string.Join(",", message));
            }

            // Save the community message database to local storage
            File.WriteAllLines("CommunityMessageDatabase.csv", communityMessageDatabaseContent);
        }

        /// <summary>
        /// Debugging function.
        /// </summary>
        public static void SaveIconographyDatabase()
        {
            List<string> icons = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "" };

            File.WriteAllLines("IconographyDatabase.csv", icons);
        }

        #endregion

        #region Complex Database Loading

        /// <summary>
        /// Finds the performance standards for a subject's course.
        /// </summary>
        /// <returns>The course's performance standards</returns>
        public static List<string> FindSubjectPerformanceStandards(string selectedCourse)
        {
            List<List<string>> courseDatabase = LoadCourseDatabase();
            List<List<string>> subjectDatabase = LoadSubjectDatabase();

            // Intialise variables to store the currently selected course's subject code and its subjects performance standards
            string courseSubjectCode = string.Empty;
            List<string> subjectPerformanceStandards = new List<string>();

            // For each course in the course database...
            foreach (List<string> course in courseDatabase)
            {
                // If the course's name is the same as the currently selected course...
                if (course[(int)CProp.Name] == selectedCourse)
                {
                    // Use this course to determine the current course's subject code
                    courseSubjectCode = course[(int)CProp.SubjectCode];
                }
            }

            // For each subject in the subject database...
            foreach (List<string> subject in subjectDatabase)
            {
                // If the subject's code is the same as the current course's subject code...
                if (subject[(int)SProp.SubjectCode] == courseSubjectCode)
                {
                    // Use this code to determine the current subject (and course's) performance standards, eliminating empty entries
                    subjectPerformanceStandards = subject[(int)SProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
            }

            return subjectPerformanceStandards;
        }

        public static string FindCourseIcon(List<string> course)
        {
            // Load the subject database
            List<List<string>> subjectDatabase = LoadSubjectDatabase();

            // For each subject in the subject database
            foreach (List<string> subject in subjectDatabase)
            {
                if (subject[(int)SProp.SubjectCode] == course[(int)CProp.SubjectCode])
                {
                    return subject[(int)SProp.Icon];
                }
            }

            return "";
        }

        public static string FindAssignmentIcon(List<string> assignment)
        {
            // Load the assessment type database
            List<List<string>> assessmentTypeDatabase = LoadAssessmentTypeDatabase();

            // For each assessment type in the assessment type database
            foreach (List<string> assessmentType in assessmentTypeDatabase)
            {
                if (assessmentType[(int)ATProp.Code] == assignment[(int)AProp.AssessmentType])
                {
                    return assessmentType[(int)ATProp.Icon];
                }
            }

            return "";
        }
    }

    #endregion

    /// <summary>
    /// A series of aggregators for database management.
    /// </summary>
    public static class DatabaseAggregator
    {
        public static Action OnCourseCreated;
        public static Action OnCourseDeleted;
        public static Action OnAssignmentCreated;
        public static Action OnAssignmentDeleted;
        public static Action OnMessageCreated;
        public static Action OnMessageDeleted;

        /// <summary>
        /// The event broadcasted when a course is created.
        /// </summary>
        public static void BroadcastCourseCreation()
        {
            OnCourseCreated?.Invoke();
        }

        /// <summary>
        /// The event broadcasted when a course is deleted.
        /// </summary>
        public static void BroadcastCourseDeletion()
        {
            OnCourseDeleted?.Invoke();
        }

        /// <summary>
        /// The event broadcasted when an assignment is created.
        /// </summary>
        public static void BroadcastAssignmentCreation()
        {
            OnAssignmentDeleted?.Invoke();
        }

        /// <summary>
        /// The event broadcasted when an assignment is deleted.
        /// </summary>
        public static void BroadcastAssignmentDeletion()
        {
            OnAssignmentDeleted?.Invoke();
        }

        /// <summary>
        /// The event broadcasted when a message is created.
        /// </summary>
        public static void BroadcastMessageCreation()
        {
            OnMessageCreated?.Invoke();
        }

        /// <summary>
        /// The event broadcasted when a message is deleted.
        /// </summary>
        public static void BroadcastMessageDeletion()
        {
            OnMessageDeleted?.Invoke();
        }
    }
}