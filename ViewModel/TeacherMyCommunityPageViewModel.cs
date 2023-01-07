using SACEology.ViewModel.Base;
using SACEology.Properties;
using System.Windows.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace SACEology.ViewModel
{
    class TeacherMyCommunityPageViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of all subject names, and hence all chat room.
        /// </summary>
        public List<string> Subjects { get; set; } = DatabaseHelpers.LoadSubjectDatabase().Select(array => array[(int)SProp.Name]).ToList();

        /// <summary>
        /// The subject of the currently selected chatroom.
        /// </summary>
        public string _subject { get; set; } = "Accounting";

        /// <summary>
        /// The subject of the currently selected chatroom.
        /// </summary>
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
                Settings.Default.SelectedSubject = _subject;
                Settings.Default.Save();

                DisplaySavedMessages();
            }
        }

        /// <summary>
        /// The messages for this currently selected chatroom.
        /// </summary>
        public ObservableCollection<MessageItemViewModel> Messages { get; set; } = new ObservableCollection<MessageItemViewModel>();

        /// <summary>
        /// The content of the IsAttachmentsOnly button
        /// </summary>
        public string IsAttachmentsOnlyButtonContent { get; set; }

        /// <summary>
        /// A private string to filter messages by
        /// </summary>
        public string _searchString { get; set; } = string.Empty;

        /// <summary>
        /// The string to filter messages by
        /// </summary>
        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                _searchString = value;
                DisplaySavedMessages();
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the scribing button is pressed, to send a new message.
        /// </summary>
        public ICommand SendMessageButtonCommand { get; set; }

        /// <summary>
        /// The command for when the "Attachments Only" button is pressed, to invert the attachments only setting
        /// </summary>
        public ICommand InvertAttachmentsOnlySettingCommand { get; set; }

        #endregion 

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="window"></param>
        public TeacherMyCommunityPageViewModel()
        {
            // Initialise button relay commands
            SendMessageButtonCommand = new RelayCommand(() => ShowSendMessagePopUp());
            NavigateBackwardButtonCommand = new RelayCommand(() => NavigateBackward());
            HelpCommand = new RelayCommand(() => ShowHelpPopUp());
            InvertAttachmentsOnlySettingCommand = new RelayCommand(() => InvertAttachmentsOnlySetting());

            // When a message is created, refresh the displayed messages
            DatabaseAggregator.OnMessageCreated += DisplaySavedMessages;

            // Populate the page with the initally loaded message
            DisplaySavedMessages();

            // Set the attachment only button's initial content 
            InvertAttachmentsOnlySetting();

            Settings.Default.SelectedSubject = "Accounting";
            Settings.Default.Save();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Loads and displays the saved messages from the message database.
        /// </summary>
        private void DisplaySavedMessages()
        {
            // Clear the messages appearance
            Messages.Clear();

            // Load the community messages database
            List<List<string>> messageDatabase = DatabaseHelpers.LoadCommunityMessageDatabase();

            Console.WriteLine("Database Length: " + messageDatabase.Count);

            // For each message in the message database...
            foreach (List<string> message in messageDatabase)
            {
                // If the message's subject is the same as the current selected chatroom's subject...
                if (message[(int)CMProp.Subject] == Subject)
                {
                    // If the message has an attachment, or if all messages should be shown anyway...
                    if (!string.IsNullOrEmpty(message[(int)CMProp.Attachment]) || Settings.Default.IsAttachmentsOnly == false)
                    {
                        // Do not include the message by search, by default, if a string has been entered
                        bool searchInclusion = string.IsNullOrEmpty(SearchString);

                        // If a string is being searched for...
                        if (!searchInclusion)
                        {
                            // Initialise a list containing the message subject, name and attahcment location, which can be hit by the search
                            List<string> searchableProperties = new List<string> { message[(int)CMProp.Subject].ToLower(), message[(int)CMProp.Name].ToLower(), message[(int)CMProp.Attachment].ToLower() };

                            // For each property in the searhable properties
                            foreach (string property in searchableProperties)
                            {
                                // If the current property contains the search string, count the current message as included
                                if (property.Contains(SearchString.ToLower())) { searchInclusion = true; }
                            }
                        }

                        if (searchInclusion) { DisplayMessage(message); };
                    }
                }
            }
        }

        /// <summary>
        /// Displays a saved message in the view.
        /// </summary>
        /// <param name="message">The message's properties</param>
        private void DisplayMessage(List<string> message)
        {
            // Initialise a variable to store the message sender's name
            string name = message[(int)CMProp.Name];

            // If a name is not given, set the name to anonymous [FIX LATER]
            if (string.IsNullOrWhiteSpace(message[(int)CMProp.Name]))
            {
                name = "Anonymous";
            }

            // Initialise a variable to store whether the message is sent by a teacher (it always will)
            bool isStudent = true;

            // Create a new message with all the passed properties
            MessageItemViewModel addedMessage = new MessageItemViewModel
            {
                IsStudent = isStudent,
                Name = name,
                Content = message[(int)CMProp.Content],
                RecordingDirectory = message[(int)CMProp.Audio],
                HasAudio = !string.IsNullOrWhiteSpace(message[(int)CMProp.Audio]),
                AttachmentDirectory = message[(int)CMProp.Attachment],
                HasAttachment = !string.IsNullOrWhiteSpace(message[(int)CMProp.Attachment]),
                Date = message[(int)CMProp.Date],
                MessageMetadata = message[(int)CMProp.Date] + " " + !isStudent

            };

            // Add this message to the collection of messages
            Messages.Add(addedMessage);
        }

        /// <summary>
        /// Inverts the attachments only setting.
        /// </summary>
        private void InvertAttachmentsOnlySetting()
        {
            // Flip the IsAttachmentsOnly setting
            Settings.Default.IsAttachmentsOnly = !Settings.Default.IsAttachmentsOnly;

            // Save the setting
            Settings.Default.Save();

            // Set the IsAttachmentOnly button's content
            UpdateIsAttachmentsOnlyButtonContent();

            // Refresh the saved messages
            DisplaySavedMessages();
        }

        /// <summary>
        /// Sets the content of the attachment only button, based on the current setting.
        /// </summary>
        private void UpdateIsAttachmentsOnlyButtonContent()
        {
            IsAttachmentsOnlyButtonContent = Settings.Default.IsAttachmentsOnly ? "Attachments Only" : "All Messages";
        }

        #endregion

        #region Pop-Up Core

        /// <summary>
        /// Shows the pop up to send a message to the community message page.
        /// </summary>
        private void ShowSendMessagePopUp()
        {
            if (!Settings.Default.PopUpOpen)
            {
                PopUpAggregator.BroadcastSendMessagePopUpCreation();
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
            Settings.Default.CurrentPage = Convert.ToInt32(ApplicationPage.TeacherPortal);

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
