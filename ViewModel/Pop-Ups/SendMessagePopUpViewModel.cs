using SACEology.ViewModel.Base;
using System.Windows.Input;
using SACEology;
using SACEology.Properties;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;

class SendMessagePopUpViewModel : BaseViewModel
{
    #region Public Properties

    /// <summary>
    /// The status of the message's author (student or teacher)
    /// </summary>
    public UserStatus Status { get; set; } = (UserStatus)Settings.Default.UserStatus;

    /// <summary>
    /// The message's parent course.
    /// </summary>
    public string Course { get; set; } = Settings.Default.SelectedCourse;

    /// <summary>
    /// The message's assignment course.
    /// </summary>
    public string Assignment { get; set; } = Settings.Default.SelectedAssignment;

    /// <summary>
    /// The message's parent subject.
    /// </summary>
    public string Subject { get; set; } = Settings.Default.SelectedSubject;

    /// <summary>
    /// The name of the person who sent the message.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The message's content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The state of the pop-up's dynamic audio management button.
    /// </summary>
    public RecordingButtonState RecordingButtonState { get; set; } = RecordingButtonState.Record;

    /// <summary>
    /// The message's audio file's name.
    /// </summary>
    public string RecordingName { get; set; }

    /// <summary>
    /// The message's audio file's directory
    /// </summary>
    public string RecordingDirectory { get; set; }

    /// <summary>
    /// The message's attachment's file name.
    /// </summary>
    public string AttachmentName { get; set; }

    /// <summary>
    /// The message's attachment's file directory.
    /// </summary>
    public string AttachmentDirectory { get; set; }

    #endregion

    #region Public Commands

    /// <summary>
    /// The command for when the "Send Message" button is pressed, to save the message.
    /// </summary>
    public ICommand SendMessageCommand { get; set; }

    /// <summary>
    /// The command for when the changing audio button is pressed, to execute an audio recording action.
    /// </summary>
    public ICommand MultipurposeAudioCommand { get; set; }

    /// <summary>
    /// The command for when the bin button is pressed, to remove the recording
    /// </summary>
    public ICommand RemoveRecordingCommand { get; set; }

    /// <summary>
    /// The command for when the upload button is pressed, to select an attachment
    /// </summary>
    public ICommand SelectAttachmentCommand { get; set; }

    /// <summary>
    /// The command for when the bin button is pressed, to remove the attachment
    /// </summary>
    public ICommand RemoveAttachmentCommand { get; set; }

    #endregion 

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public SendMessagePopUpViewModel()
    {
        // Connect relay commands
        SendMessageCommand = new RelayCommand(() => ValidateAssignmentCreation());
        MultipurposeAudioCommand = new RelayCommand(() => InterpretAudioButtonCommand());
        RemoveRecordingCommand = new RelayCommand(() => RemoveAudio());
        SelectAttachmentCommand = new RelayCommand(() => SelectAttachment());
        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Imports the Windows MIME to record and save the audio.
    /// </summary>
    /// <param name="lpstrCommand">The command to open WinMIME</param>
    /// <param name="lpstrReturnString">The command to return data from WinMIME</param>
    /// <param name="uReturnLength">WinMIMEs string return length</param>
    /// <param name="hwndCallback">WinMIMEs callback</param>
    /// <returns></returns>
    [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    private static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

    /// <summary>
    /// Run a function depending on the aduio button's state
    /// </summary>
    private void InterpretAudioButtonCommand()
    {
        switch (RecordingButtonState)
        {
            // If the button is in the record states...
            case RecordingButtonState.Record:
                BeginRecordingAudio();
                break;

            // If the button is in the stop state...
            case RecordingButtonState.Stop:
                EndRecordingAudio();
                break;

            // If the button is in the play state...
            case RecordingButtonState.Play:
                PlayAudio();
                break;

            // If the button is in the pause state...
            case RecordingButtonState.Pause:
                PauseAudio();
                break;
        }
    }

    /// <summary>
    /// Uses <see cref="mciSendString(string, string, int, int)"/> to begin recording an audio file for this message. 
    /// </summary>
    private void BeginRecordingAudio()
    {
        mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
        mciSendString("record recsound", "", 0, 0);

        RecordingButtonState = RecordingButtonState.Stop;
    }


    /// <summary>
    /// Uses <see cref="mciSendString(string, string, int, int)"/> to stop recording an audio file for this message. 
    /// </summary>
    private void EndRecordingAudio()
    {
        // Regenerate the recording's name 
        GenerateRecordingDirectory();

        // Save sound recording 
        mciSendString("save recsound " + RecordingDirectory, "", 0, 0);
        mciSendString("close recsound ", "", 0, 0);

        // Display that the audio can be played
        RecordingButtonState = RecordingButtonState.Play;
    }

    /// <summary>
    /// Uses <see cref="mciSendString(string, string, int, int)"/> to play this message's audio file.
    /// </summary>
    private void PlayAudio()
    {
        mciSendString("play " + RecordingDirectory, null, 0, 0);
        RecordingButtonState = RecordingButtonState.Pause;
    }

    /// <summary>
    /// Uses <see cref="mciSendString(string, string, int, int)"/> to pause this message's audio file.
    /// </summary>
    private void PauseAudio()
    {
        mciSendString("pause " + RecordingDirectory, null, 0, 0);
        RecordingButtonState = RecordingButtonState.Play;
    }

    /// <summary>
    /// Clears this message's audio file
    /// </summary>
    private void RemoveAudio()
    {
        RecordingName = string.Empty;
        RecordingButtonState = RecordingButtonState.Record;
    }

    /// <summary>
    /// Opens a file dialog for the user to select an attachment.
    /// </summary>
    private void SelectAttachment()
    {
        // Initialise a new file dialog
        OpenFileDialog openFileDialog = new OpenFileDialog();

        // Load the file dialog in the user's last navigated folder
        openFileDialog.RestoreDirectory = true;

        // If a file was selected...
        if (openFileDialog.ShowDialog() == true)
        {
            // Set the attachment's file name and directory to the specified location
            AttachmentDirectory = openFileDialog.FileName;
            AttachmentName = AttachmentDirectory.Split('\\').Last();
        }
    }

    /// <summary>
    /// Clear the message's attachment name and directory, effectivley removing it from the message
    /// </summary>
    private void RemoveAttachment()
    {
        // Clear the attachment name and directory
        AttachmentDirectory = string.Empty;
        AttachmentName = string.Empty;
    }

    /// <summary>
    /// Generates the recording's name, with its course, assignment and unique ID.
    /// </summary>
    private void GenerateRecordingDirectory()
    {
        // Truncate the first and second sections of the audio's name with the first ten characters of the course's name, if applicable.
        string courseComponent = Course.Length > 10 ? Course.Substring(0, 10) : Course;
        string assignmentComponent = Assignment.Length > 10 ? Assignment.Substring(0, 10) : Assignment;

        RecordingName = courseComponent + "_" + assignmentComponent + "_" + Settings.Default.AudioID.ToString() + ".wav";
        RecordingDirectory = "C:/Users/THOMASF001/Desktop/Audio/" + RecordingName;

        Settings.Default.AudioID++;
        Settings.Default.Save();
    }

    private void ValidateAssignmentCreation()
    {
        // If the message has been given content, send it
        if (!string.IsNullOrWhiteSpace(Content))
        {
            SendMessage();
        }
        // Otherwise, remind the user to give their message content
        else
        {
            PopUpAggregator.BroadcastErrorPopUpCreation("Please enter this message's content.");
        }
    }

    /// <summary>
    /// Posts the message, saving it to the database and reopening the messages pop up.
    /// </summary>
    private void SendMessage()
    {
        if (Settings.Default.SelectedMessageServer == (int)MessageServer.Assignment)
        {
            // Create a message with all the nessecary parameters
            List<string> message = new List<string>
            {
                ((int)Status).ToString(), Course, Assignment,
                Name, Content, DateTime.Now.ToString("d/M"), DateTime.Now.ToString("HH:mm"),
                RecordingDirectory, AttachmentDirectory
            };

            // Add this message to the database
            DatabaseHelpers.AppendAssignmentMessageToDatabase(message);

            // A message creation does not need to be broadcast, as the messages pop-up is reloaded

            // Close the pop up and open the messages pop up
            PopUpAggregator.BroadcastMessagesPopUpCreation();

            // Confirm that the message has sent
            PopUpAggregator.BroadcastConfirmationPopUpCreation("Message successfully posted.");
        }
        else
        {
            // Create a message with all the nessecary parameters
            List<string> message = new List<string>
            {
                Subject,
                Name, Content, DateTime.Now.ToString("d/M"), DateTime.Now.ToString("HH:mm"),
                RecordingDirectory, AttachmentDirectory
            };

            // Add this message to the database
            DatabaseHelpers.AppendCommunityMessageToDatabase(message);

            // Broadcast that a message has been created
            DatabaseAggregator.BroadcastMessageCreation();

            // Close the pop up
            ClosePopUp();

            // Confirm that the message has sent
            PopUpAggregator.BroadcastConfirmationPopUpCreation("Message successfully posted.");
        }
    }

    #endregion

    #region Pop-Up Core

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
