using SACEology.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SACEology
{
    public static class ValidationHelpers
    {
        /// <summary>
        /// Truncates strings to ensure they don't overflow within controls.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The appropriatley truncated string</returns>
        public static string TruncateString(string input)
        {
            // The number of characters to truncate
            int truncationLength = 40;

            // Return a truncated string if the input is above 40 characters, return the original string otherwise
            return input.Length > truncationLength ? input.Substring(0, truncationLength) + "..." : input;
        }

        #region Assignment Property Validation

        /// <summary>
        /// Ensures that a datetime is valid, and returns the current date otherwise.
        /// </summary>
        /// <param name="input">The entered date</param>
        /// <returns>The validated date</returns>
        public static DateTime ValidateDate(string input)
        {
            // Intialise a new DateTime to store the entered date, if valid...
            DateTime result;

            // If the entered date passes as a DateTime variable successfully...
            if (DateTime.TryParse(input, out result))
            {
                // Return the entered date as a DateTime
                return DateTime.Parse(input);
            }
            else
            {
                // Return today's date as the default
                return DateTime.Now;  
            }
        }

        /// <summary>
        /// Takes the starting date and returns its valid, cleaned string version that lies before the due date.
        /// </summary>
        /// <param name="sd">The starting date</param>
        /// <param name="dd">The due date</param>
        /// <returns>The cleaned starting date</returns>
        public static string ValidateStartingDate(string sd, string dd)
        {
            // Convert the entered starting date into a DateTime
            DateTime startDate = ValidateDate(sd);

            // Convert the entered due date into a DateTime
            DateTime dueDate = ValidateDate(dd);

            // If the starting date is before the due date
            if (dueDate < startDate)
            {
                // Return the due date as the starting date (since it cannot be after when it is due) 
                return dueDate.ToString("d/M");
            }
            else
            {
                // Return the entered starting date
                return startDate.ToString("d/M");
            }
        }

        /// <summary>
        /// Takes the due date and returns its valid, cleaned string version that lies after the starting date.
        /// </summary>
        /// <param name="sd">The starting date</param>
        /// <param name="dd">The due date</param>
        /// <returns>The cleaned starting date</returns>
        public static string ValidateDueDate(string sd, string dd)
        {
            // Interpret the passes starting and due dates as dates
            DateTime startDate = ValidateDate(sd);
            DateTime dueDate = ValidateDate(dd);

            // If the starting date is past the due date...
            if (startDate > dueDate)
            {
                // Change the due date the starting date (as it cannot be after the due date) 
                return startDate.ToString("d/M");
            }
            else
            {
                // Return the entered due date
                return dueDate.ToString("d/M");
            }
        }

        /// <summary>
        /// Takes an assignment's weight and returns the closest valid weight possible.
        /// </summary>
        /// <param name="course">The assignment's course</param>
        /// <param name="input">The assignment's weight</param>
        /// <returns>The assignment's cleaned weight</returns>
        public static string ValidateWeight(string course, string input)
        {
            // Initialise a new integer variable for the assignement's current weight
            int weight;

            // Initialise a new integer variable for the sum of the weights of assignments across the course
            int weightSum = 0;

            // Load the assignment database...
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // For each assignment in the assignment database...
            foreach (List<string> assessment in assignmentDatabase)
            {
                // If the current assignment belongs to the current course...
                if (assessment[(int)AProp.Course] == course)
                {
                    // Add the current assignment's weight to the weightSum
                    weightSum += Convert.ToInt32(assessment[(int)AProp.Weight]);
                }
            }

            // 100 - weightSum equals the maximum weight that can be allocated to this assignment
            int weightLimit = 100 - weightSum;

            // Try to interpret the input as an integer, represeting the assignment's weight
            if (int.TryParse(input, out weight))
            {
                // If the assignment's entered weight is above the weight limit, bring it back down to this limit
                if (weight > weightLimit) { return Convert.ToString(weightLimit); }

                // If the assignment's entered weight is below zero, bring it up to zero
                else if (weight < 0 ) { return Convert.ToString(0); }

                // Return the entered weight
                else { return Convert.ToString(weight); }
            }
            // If a valid integer has not been input...
            else
            {
                // Set the assignment's weight to zero
                return Convert.ToString(0);
            }
        }

        #endregion
        
        /// <summary>
        /// Converts a letter grade (A+, A, A-...) to its numeric value
        /// </summary>
        /// <param name="grade">The letter grade</param>
        /// <returns>The letter grade's numeric value</returns>
        public static int LetterToNumericGrade(string grade)
        {
            // Use a hard-coded table to determine which value is being refferred to, while solving for multiple cases
            switch (grade)
            {
                case "A+":
                case "A +":
                case "a+":
                case "a +":
                    return 15;

                case "A":
                case "a":
                case "Excellent":
                    return 14;

                case "A-":
                case "A -":
                case "a-":
                case "a -":
                    return 13;

                case "B+":
                case "B +":
                case "b+":
                case "b +":
                    return 12;

                case "B":
                case "b":
                case "Good":
                    return 11;

                case "B-":
                case "B -":
                case "b-":
                case "b -":
                    return 10;

                case "C+":
                case "C +":
                case "c+":
                case "c +":
                    return 9;

                case "C":
                case "c":
                case "Satisfactory":
                    return 8;

                case "C-":
                case "C -":
                case "c-":
                case "c -":
                    return 7;

                case "D+":
                case "D +":
                case "d+":
                case "d +":
                    return 6;

                case "D":
                case "d":
                case "Poor":
                    return 5;

                case "D-":
                case "D -":
                case "d-":
                case "d -":
                    return 4;

                case "E+":
                case "E +":
                case "e+":
                case "e +":
                    return 3;

                case "E":
                case "e":
                case "Minimal":
                    return 2;

                case "E-":
                case "E -":
                case "e-":
                case "e -":
                    return 1;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts a numeric value to its letter grade (A+, A, A-...)
        /// </summary>
        /// <param name="grade">The numeric grade</param>
        /// <returns>The numeric grade's letter representation</returns>
        public static string NumericToLetterGrade(double grade)
        {
            // If the entered grade is valid...
            if (grade > 1 || grade < -15)
            {
                // Round the entered grade to its nearest integer value
                int roundedGrade = Convert.ToInt16(Math.Round(grade));

                // Use a table of values to determine the numeric grade's corresponding notation
                switch (roundedGrade)
                {
                    case 15:
                        return "A+";

                    case 14:
                        return "A";

                    case 13:
                        return "A-";

                    case 12:
                        return "B+";

                    case 11:
                        return "B";

                    case 10:
                        return "B-";

                    case 9:
                        return "C+";

                    case 8:
                        return "C";

                    case 7:
                        return "C-";

                    case 6:
                        return "D+";

                    case 5:
                        return "D";

                    case 4:
                        return "D-";

                    case 3:
                        return "E+";

                    case 2:
                        return "E";

                    case 1:
                        return "E-";

                    default:
                        return "N/A";
                }
            }
            // Otherwise, return N/A
            else
            {
                return "N/A";
            }
        }

        /// <summary>
        /// Converts a numeric grade to its combination numeric and letter grades, its grade as displayed in the view.
        /// </summary>
        /// <param name="grade">The numeric grade</param>
        /// <returns>The numeric grade's displayed representation</returns>
        public static string NumericToDisplayGrade(double grade)
        {
            // Find this numeric grade's corresponding letter grade
            string letterGrade = NumericToLetterGrade(grade);

            // Return these grades as a display
            return grade + " (" + letterGrade + ")";
        }

        /// <summary>
        /// Ensures that an entered grade is a valid grade from 1 to 15, in numeric form.
        /// </summary>
        /// <param name="input">The entered grade</param>
        /// <returns>The grade's value (or nothing)</returns>
        public static string ValidateGrade(string input)
        {
            // Initialise a variable for this assignment's grade value
            double gradeValue;

            // Test whether a valid number has been input for this assignment, and clamp it to between 1 and 15 (the valid range of grades)
            bool isNumeric = double.TryParse(input, out gradeValue);
            gradeValue = Math.Min(Math.Max(1, gradeValue), 15);

            // If a number has not been input...
            if (!isNumeric)
            {
                // Attempt to parse the input grade as a string, returning 0 if it is still invalid
                gradeValue = LetterToNumericGrade(input);
            }

            // Return the grade, unless a 0 has been returned, in which case return nothing
            return gradeValue != 0 ? gradeValue.ToString() : string.Empty;
        }


        #region Locators

        /// <summary>
        /// Finds which performance standards are currently missing within a course's syllabus.
        /// </summary>
        /// <param name="course">The course's name</param>
        /// <returns>The course's missing performance standards</returns>
        public static List<string> FindMissingPerformanceStandards(string course)
        {
            // Load the subject and assignment databases
            List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Initiliase a new list of strings, storing the missing performance standards
            List<string> missingPerformanceStandards = new List<string>();

            // For each subject in the subject database...
            foreach (List<string> subject in subjectDatabase)
            {
                // If the current subject's name matches the current subject of their course
                if (subject[(int)SProp.Name] == Settings.Default.SelectedSubject)
                {
                    // Load the current subject's performance standards into missingPerformanceStandards, delimited by spaces
                    missingPerformanceStandards = subject[(int)SProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }

            // For each assignment in the assignment database...
            foreach (List<string> assignment in assignmentDatabase)
            {
                // If the assignment's name matches the current assignment, passed into the function...
                if (assignment[(int)AProp.Course] == course)
                {
                    // Create a new list of strings, performanceStandards, from performance standards from this assignment
                    List<string> performanceStandards = assignment[(int)AProp.PerformanceStandards].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    // For each performance standard in this assignment...
                    foreach (string standard in performanceStandards)
                    {
                        // Remove the current performance standard from the list of missing performance standards, if applicable
                        missingPerformanceStandards.Remove(standard);
                    }
                }
            }

            return missingPerformanceStandards;
        }

        /// <summary>
        /// Finds which assessment types are currently missing within a course's syllabus.
        /// </summary>
        /// <param name="name">The course's name</param>
        /// <returns>The course's missing performance standards</returns>
        public static List<string> FindMissingAssessmentTypes(string name)
        {
            // Load the subject and assignment databases
            List<List<string>> subjectDatabase = DatabaseHelpers.LoadSubjectDatabase();
            List<List<string>> assignmentDatabase = DatabaseHelpers.LoadAssignmentDatabase();

            // Initiliase new lists for missing assessment types
            List<string> missingAssessmentTypes = new List<string>();

            // For each subject in the subject database...
            foreach (List<string> subject in subjectDatabase)
            {
                // If the current subject's name matches the current subject of their course
                if (subject[(int)SProp.Name] == Settings.Default.SelectedSubject)
                {
                    // Load the current subject's assessment types into missingAssessmentTypes, delimited by spaces
                    missingAssessmentTypes = subject[(int)SProp.AssessmentTypes].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }

            // For each assignment in the assignment database...
            foreach (List<string> assignment in assignmentDatabase)
            {
                // If the assignment's name matches the current assignment, passed into the function...
                if (assignment[(int)AProp.Course] == name)
                {
                    // Find this assignment's assessment types and remove it from missingAssessmentTypes
                    string assessmentType = assignment[(int)AProp.AssessmentType];
                    missingAssessmentTypes.Remove(assessmentType);
                }
            }

            return missingAssessmentTypes;
        }

        #endregion

        /// <summary>
        /// A series of aggregators for database management.
        /// </summary>
        public static class ValidationAggregator
        {
            public static Action OnGradeEdited;

            public static void BroadcastGradeEditing()
            {
                OnGradeEdited?.Invoke();
            }
        }
    }
}