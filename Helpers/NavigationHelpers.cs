using SACEology.Properties;
using System;
using System.Collections.Generic;

namespace SACEology
{
    /// <summary>
    /// Navigates the user to their previously accessed page (osbolete).
    /// </summary>
    public static class BackButtonAggregator
    {
        public static Action OnNavigation;

        public static void BroadcastNavigation()
        {
            OnNavigation?.Invoke();
        }
    }

    /// <summary>
    /// A series of aggregators for pop ups.
    /// </summary>
    public class PopUpAggregator
    {
        // Initialise public events to be received when a pop-up is created or deleted.
        public static Action OnCreditsPopUpCreation;
        public static Action OnSettingsPopUpCreation;
        public static Action OnAddCoursePopUpCreation;
        public static Action OnAddAssessmentPopUpCreation;
        public static Action OnSendMessagePopUpCreation;
        public static Action OnMessagesPopUpCreation;
        public static Action<List<string>, List<double>, List<double>> OnGradeBreakdownPopUpCreation;
        public static Action OnCourseGoalSeekerPopUpCreation;
        public static Action OnHelpPopUpCreation;
        public static Action OnDeletion;
        public static Action<string> OnErrorPopUpCreation;
        public static Action<string> OnConfirmationPopUpCreation;


        /// <summary>
        /// Broadcasts that an "Credits" pop-up has been created.
        /// </summary>
        public static void BroadcastCreditsPopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnCreditsPopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that an "Settings" pop-up has been created.
        /// </summary>
        public static void BroadcastSettingsPopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnSettingsPopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that an "Add Course" pop-up has been created.
        /// </summary>
        public static void BroadcastAddCoursePopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnAddCoursePopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that an "Add Assessment" pop-up has been created.
        /// </summary>
        public static void BroadcastAddAssessmentPopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnAddAssessmentPopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that a "Send Message" pop-up has been created.
        /// </summary>
        public static void BroadcastSendMessagePopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnSendMessagePopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that a "Messages" pop-up has been created.
        /// </summary>
        public static void BroadcastMessagesPopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnMessagesPopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that a "Grade Breakdown" pop-up has been created.
        /// </summary>
        /// <param name="standards">The entered set of perofrmance stnadards</param>
        /// <param name="occurences">The calculated range of performance standard occurences</param>
        /// <param name="grades">The calculated performance standard grades</param>
        public static void BroadcastGradeBreakdownPopUpCreation(List<string> standards, List<double> occurences, List<double> grades)
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnGradeBreakdownPopUpCreation?.Invoke(standards, occurences, grades);
        }

        /// <summary>
        /// Broadcasts that a "Goal Seeker" pop-up has been created.
        /// </summary>
        public static void BroadcastCourseGoalSeekerPopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnCourseGoalSeekerPopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that a "Help" pop-up has been created
        /// </summary>
        public static void BroadcastHelpPopUpCreation()
        {
            // Close any open pop-ups
            BroadcastDeletion();

            // Save that a pop-up has been opened
            Settings.Default.PopUpOpen = true;
            Settings.Default.Save();

            // Call the event to open this pop-up
            OnHelpPopUpCreation?.Invoke();
        }

        /// <summary>
        /// Broadcasts that a pop-up has been deleted.
        /// </summary>
        public static void BroadcastDeletion()
        {
            // Save that a pop-up has been closed
            Settings.Default.PopUpOpen = false;
            Settings.Default.Save();

            // Call the event to close ANY active pop-up
            OnDeletion?.Invoke();
        }

        /// <summary>
        /// Broadcasts that an error pop-up has been created.
        /// </summary>
        /// <param name="message">The error pop-up's message</param>
        public static void BroadcastErrorPopUpCreation(string message)
        {
            // Call the event to open a new error pop-up, with the appropriate message
            OnErrorPopUpCreation?.Invoke(message);
        }

        /// <summary>
        /// Broadcasts that a confirmation pop-up has been created.
        /// </summary>
        /// <param name="message">The confirmation pop-up's message</param>
        public static void BroadcastConfirmationPopUpCreation(string message)
        {
            // Call the event to open a new confirmation pop-up, with the appropriate message
            OnConfirmationPopUpCreation?.Invoke(message);
        }
    }
}