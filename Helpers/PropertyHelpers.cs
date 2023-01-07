using System;
using System.Collections.Generic;
using System.Linq;

namespace SACEology
{
    public static class PropertyHelpers
    {
        /// <summary>
        /// Determines whether a course has the soonest-due assignment.
        /// </summary>
        /// <param name="name">The course's name</param>
        /// <returns>Whether the course has the soonest-due assignment.</returns>
        public static bool HasSoonestAssignment(string name)
        {
            // Load the assignment database
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Create a variable to store the most urgent assignment, in the entire course
            List<string> soonestAssignment = new List<string>();

            // If there is at least one assignment...
            if (assignmentDatabase.Count > 0)
            {
                // Begin the call loop by setting the first assignment in the app as the soonest
                soonestAssignment = assignmentDatabase[0];
            }

            // For each assignment in the assignment database...
            foreach (List<string> assignment in assignmentDatabase)
            {
                // If the current assignment is more urgent than soonestAssignment (its due date is less...)
                if (DateTime.Parse(assignment[(int)AProp.DueDate]) < DateTime.Parse(soonestAssignment[(int)AProp.DueDate]))
                {
                    // The soonest assignment is now the current assignment
                    soonestAssignment = assignment;
                }
            }

            // If this course contains the most recent assignment, return true, awarding it a badge on the ViewModel
            return name == soonestAssignment[(int)AProp.Course];
        }

        /// <summary>
        /// Determines whether a course has the most assignments.
        /// </summary>
        /// <param name="name">The course's name</param>
        /// <returns>Whether the course has the most assignments.</returns>
        public static bool IsMostPopulatedCourse(string name)
        {
            // Find the most frequent course in the assignment database
            List<string> courseOneHot = DatabaseHelpers.LoadAssignmentDatabase().Select(array => array[(int)AProp.Course]).ToList();
            string mostFrequentCourse = courseOneHot.GroupBy(i => i).OrderBy(g => g.Count()).Select(g => g.Key).ToList().Last();

            int courseCount = courseOneHot.Count(x => x.Contains(name));
            int mostFrequentCourseCount = courseOneHot.Count(x => x.Contains(mostFrequentCourse));

            // If this course is the most frequent course, return true, give it the most assignments badge
            return courseCount == mostFrequentCourseCount;
        }

        /// <summary>
        /// Returns whether an assignment is the most urgent in its course.
        /// </summary>
        /// <param name="course">The assignment's course</param>
        /// <param name="dueDate">The assignment's due date</param>
        /// <returns>Whether the assignment is the most urgent in its course</returns>
        public static bool IsSoonestAssignment(string course, string dueDate)
        {            
            // Find all assignments in the passed course
            List<List<string>> filteredAssignmentDatabase = FilterAssignmentDatabaseToCourse(course);

            // Sort assignments in the course by their due date, and find the minimum: the soonest assignment's due date
            var soonestDueDate = filteredAssignmentDatabase.Select(array => array[(int)AProp.DueDate]).Min();

            // If the assignment's passed due date is equal to the soonest assignment's due date, return true, awarding it a badge on the ViewModel
            return DateTime.Parse(dueDate) == DateTime.Parse(soonestDueDate);
        }

        /// <summary>
        /// Determines whether an assignment has the least number of performance standards.
        /// </summary>
        /// <param name="course">The assignment's course</param>
        /// <param name="packagedPerformanceStandards">The string version of the assginment's performance standards</param>
        /// <returns></returns>
        public static bool IsNarrowestAssignment(string course, string packagedPerformanceStandards)
        {
            // Initalise a database of all assignments in the course
            List<List<string>> filteredAssignmentDatabase = FilterAssignmentDatabaseToCourse(course);

            // Initialise a variable for the minimum number of performance standards in this course
            int minimumPerformanceStandards = 1000;

            // Iterate through all assignments in this course, finding the minimum number of performance standards
            foreach (List<string> assignment in filteredAssignmentDatabase)
            {
                List<string> performanceStandards = assignment[(int)AProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (performanceStandards.Count < minimumPerformanceStandards) { minimumPerformanceStandards = performanceStandards.Count; }
            }
            
            return packagedPerformanceStandards.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == minimumPerformanceStandards;
        }

        /// <summary>
        /// Determines whether an assignment has the most number of performance standards.
        /// </summary>
        /// <param name="course">The assignment's course</param>
        /// <param name="packagedPerformanceStandards">The string version of the assginment's performance standards</param>
        /// <returns></returns>
        public static bool IsBroadestAssignment(string course, string packagedPerformanceStandards)
        {
            // Initalise a database of all assignments in the course
            List<List<string>> filteredAssignmentDatabase = FilterAssignmentDatabaseToCourse(course);

            // Initialise a variable for the minimum number of performance standards in this course
            int maximumPerformanceStandards = 0;

            // Iterate through all assignments in this course, finding the minimum number of performance standards
            foreach (List<string> assignment in filteredAssignmentDatabase)
            {
                List<string> performanceStandards = assignment[(int)AProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (performanceStandards.Count > maximumPerformanceStandards) { maximumPerformanceStandards = performanceStandards.Count; }
            }

            return packagedPerformanceStandards.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length == maximumPerformanceStandards;
        }

        public static bool IsMostWeightedAssignment(string course, string weight)
        {
            // Initalise a database of all assignments in the course
            List<List<string>> filteredAssignmentDatabase = FilterAssignmentDatabaseToCourse(course);

            int maximumWeight = filteredAssignmentDatabase.Select(array => array[(int)AProp.Weight]).Select(int.Parse).ToList().Max();

            return maximumWeight == Convert.ToInt16(weight);
        }

        public static bool IsShortestLengthAssignment(string course, string startDate, string dueDate)
        {
            // Initalise a database of all assignments in the course
            List<List<string>> filteredAssignmentDatabase = FilterAssignmentDatabaseToCourse(course);

            TimeSpan duration = DateTime.Parse(dueDate) - DateTime.Parse(startDate);
            TimeSpan minimumDuration = filteredAssignmentDatabase.Select(array => (DateTime.Parse(array[(int)AProp.StartingDate]) - DateTime.Parse(array[(int)AProp.DueDate]))).ToList().Max();

            return minimumDuration == duration;
        }

        /// <summary>
        /// Returns the list of assignments that exist in the passed course.
        /// </summary>
        /// <param name="course">The current course</param>
        /// <returns>The list of assignments that exist in the current course</returns>
        public static List<List<string>> FilterAssignmentDatabaseToCourse(string course)
        {
            // Load the assignment database
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Create a new list of list of strings to store assignments that exist in the passed course
            List<List<string>> filteredAssignments = new List<List<string>>();

            // For each assignment in the assignment database...
            foreach (List<string> assignment in assignmentDatabase)
            {
                // If the current assignment's course is equal to the passed course...
                if (assignment[(int)AProp.Course] == course)
                {
                    // Add the current assignment to filteredAssignments
                    filteredAssignments.Add(assignment);
                }
            }

            return filteredAssignments;
        }
    }
}