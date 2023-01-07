using SACEology.ViewModel.Base;
using System.Windows.Input;
using SACEology;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using SACEology.Properties;
using System;

class MessagesPopUpViewModel : BaseViewModel
{
    #region Public Properties

    /// <summary>
    /// The current assignment.
    /// </summary>
    public string Assignment { get; set; } = Settings.Default.SelectedAssignment;

    /// <summary>
    /// An observable collection of the assignment's messages.
    /// </summary>
    public ObservableCollection<MessageItemViewModel> Messages { get; set; } = new ObservableCollection<MessageItemViewModel>();

    #endregion

    #region

    public ICommand SendMessageButtonCommand { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public MessagesPopUpViewModel()
    {
        DisplaySavedMessages();

        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());
        SendMessageButtonCommand = new RelayCommand(() => ShowSendMessagePopUp());

        DatabaseAggregator.OnMessageDeleted += DisplaySavedMessages;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Loads and displays the saved messages from the message database.
    /// </summary>
    private void DisplaySavedMessages()
    {
        // Clear all currently displayed messages
        Messages.Clear();

        // Load the message database
        List<List<string>> messageDatabase = DatabaseHelpers.LoadAssignmentMessageDatabase();

        // For each message in the message database
        foreach (List<string> message in messageDatabase)
        {
            // If the message's associated assignment is message's assignment
            if (message[(int)AMProp.Assignment] == Assignment)
            {
                // Display the message
                DisplayMessage(message);
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
        string name = message[(int)AMProp.Name];

        // If a name is not given, set the name to anonymous [FIX LATER]
        if (string.IsNullOrWhiteSpace(message[(int)AMProp.Name]))
        {
            name = "Anonymous";
        }

        // Initialise a variable to store whether the message is sent by a student, and set it depending on the user's status
        bool isStudent = ((UserStatus)Convert.ToInt16(message[(int)AMProp.Status]) == UserStatus.Student);

        // Create a new message with all the passed properties
        MessageItemViewModel addedMessage = new MessageItemViewModel
        {
            IsStudent = isStudent,
            Course = message[(int)AMProp.Course],
            Assignment = message[(int)AMProp.Assignment],
            Name = name,
            Content = message[(int)AMProp.Content],
            RecordingDirectory = message[(int)AMProp.Audio],
            HasAudio = !string.IsNullOrWhiteSpace(message[(int)AMProp.Audio]),
            AttachmentDirectory = message[(int)AMProp.Attachment],
            HasAttachment = !string.IsNullOrWhiteSpace(message[(int)AMProp.Attachment]),
            Date = message[(int)AMProp.Date],
            Time = message[(int)AMProp.Time],
            DisplayTime = message[(int)AMProp.Date] + ", " + message[(int)AMProp.Time],
            MessageMetadata = message[(int)AMProp.Date] + " " + !isStudent       
        };

        // Add this message to the collection of messages
        Messages.Add(addedMessage);
    }

    #endregion

    #region Pop-Up Core

    public void ShowSendMessagePopUp()
    {
        PopUpAggregator.BroadcastDeletion();
        PopUpAggregator.OnSendMessagePopUpCreation();
    }

    /// <summary>
    /// The command for when a pop-up is closed
    /// </summary>
    public ICommand ClosePopUpCommand { get; set; }

    /// <summary>
    /// Closes the current pop-up.
    /// </summary>
    private void ClosePopUp()
    {
        PopUpAggregator.BroadcastDeletion();
    }

    #endregion
}
