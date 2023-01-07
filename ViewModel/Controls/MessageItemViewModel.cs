using SACEology.ViewModel.Base;
using System.Windows.Input;
using SACEology;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

class MessageItemViewModel : BaseViewModel
{
    #region Public Properties
    
    /// <summary>
    /// If the message was sent by a student
    /// </summary>
    public bool IsStudent { get; set; }

    /// <summary>
    ///The message's course
    /// </summary>
    public string Course { get; set; }

    /// <summary>
    /// The message's assignment
    /// </summary>
    public string Assignment { get; set; }

    /// <summary>
    /// The message's author.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The message's content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The date in which this message was sent.
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// The time in which this message was sent.
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// The displayed time for the message.
    /// </summary>
    public string DisplayTime { get; set; }

    /// <summary>
    /// Whether this message has any audio file associated with it.
    /// </summary>
    public bool HasAudio { get; set; }

    /// <summary>
    /// The directory of this message's audio file.
    /// </summary>
    public string RecordingDirectory { get; set; }

    /// <summary>
    /// The state of this message's contextual audio button.
    /// </summary>
    public RecordingButtonState RecordingButtonState { get; set; } = RecordingButtonState.Play;

    /// <summary>
    /// Whether this message has any attachment assocated with it.
    /// </summary>
    public bool HasAttachment { get; set; }

    /// <summary>
    /// The directory of this message's attachment.
    /// </summary>
    public string AttachmentDirectory { get; set; }

    /// <summary>
    /// The message's metadata, to be passed into its colour converter (it's date and sending user's status)
    /// </summary>
    public string MessageMetadata { get; set; }
    
    #endregion

    #region Public Commands

    /// <summary>
    /// The command for when the changing audio button is pressed, to execute an audio recording action.
    /// </summary>
    public ICommand MultipurposeAudioCommand { get; set; }

    /// <summary>
    /// The command for when the paperclip button is pressed, to download the message's attachment.
    /// </summary>
    public ICommand DownloadAttachmentCommand { get; set; }

    /// <summary>
    /// The command for when the bin button is pressed, to remove the message
    /// </summary>
    public ICommand RemoveMessageCommand { get; set; }

    #endregion 

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public MessageItemViewModel()
    {
        MultipurposeAudioCommand = new RelayCommand(() => InterpretAudioButtonCommand());
        DownloadAttachmentCommand = new RelayCommand(() => DownloadAttachment());
        RemoveMessageCommand = new RelayCommand(() => DeleteMessage());
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
    /// Plays the audio file if it is paused, and pauses the audio file if it is playing
    /// </summary>
    private void InterpretAudioButtonCommand()
    {
        switch (RecordingButtonState)
        {
            case RecordingButtonState.Play:
                PlayAudio();
                break;

            case RecordingButtonState.Pause:
                PauseAudio();
                break;
        }
    }


    /// <summary>
    /// Pauses the currently playing audio file.
    /// </summary>
    private void PlayAudio()
    {
        mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
        mciSendString("play " + RecordingDirectory, null, 0, 0);
        RecordingButtonState = RecordingButtonState.Pause;
    }

    /// <summary>
    /// Plays the currently paused audio file.
    /// </summary>
    private void PauseAudio()
    {
        mciSendString("pause " + RecordingDirectory, null, 0, 0);
        RecordingButtonState = RecordingButtonState.Play;
    }

    private void DeleteMessage()
    {
        // Load the master database of message
        List<List<string>> messageDatabase = DatabaseHelpers.LoadAssignmentMessageDatabase();

        int removedMessageIndex = 0;

        // Find all occurences of the course across the course and assignment databases
        foreach (int i in Enumerable.Range(0, messageDatabase.Count))
        {
            if (messageDatabase[i][(int)AMProp.Course] == Course && 
                messageDatabase[i][(int)AMProp.Assignment] == Assignment && 
                messageDatabase[i][(int)AMProp.Content] == Content)
            {
                removedMessageIndex = i;
            }
        }

        // Remove the message
        messageDatabase.RemoveAt(removedMessageIndex);

        // Save the message database with the newly deleted message
        DatabaseHelpers.SaveAssignmentMessageDatabase(messageDatabase);
        DatabaseAggregator.BroadcastMessageDeletion();
    }

    /// <summary>
    /// Downloads the message's attachment to the downloads folder.
    /// </summary>
    private void DownloadAttachment()
    {
        // Split the attachemnt directory into its constutient folders
        string[] attachmentDirectoryParts = AttachmentDirectory.Split('\\');

        // Initialise the attachment's name as the last item of attahcment parts, which will always be the file name
        string attachmentName = attachmentDirectoryParts[attachmentDirectoryParts.Length - 1];

        // Attempt to download the file to the user's documents folder
        try
        {
            // Copy the message's attached file, from its original directory, to the user's documents folder
            File.Copy(AttachmentDirectory, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), attachmentName));

            // Confirm that the user has successfully downloaded their file by creating a pop-up confirmation
            PopUpAggregator.BroadcastConfirmationPopUpCreation(attachmentName + " successfully downloaded to your Documents folder.");
        }
        // If the user has already downloaded the attachment...
        catch
        {
            // Confirm that the user has successfully downloaded their file by creating a pop-up confirmation
            PopUpAggregator.BroadcastErrorPopUpCreation("You've already downloaded " + attachmentName + " to your Documents folder.");
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
