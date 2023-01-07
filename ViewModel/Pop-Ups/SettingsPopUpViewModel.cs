using SACEology.ViewModel.Base;
using System.Windows.Input;
using System.Linq;
using SACEology;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SACEology.ViewModel;
using System;
using SACEology.Properties;

class SettingsPopViewModel : BaseViewModel
{
    #region Public Properties

    /// <summary>
    /// The content of the auto-delete messages setting button.
    /// </summary>
    public string AutoDeleteMessagesSettingButtonContent { get; set; }

    /// <summary>
    /// The content of the scale ATAR grades setting button.
    /// </summary>
    public string ScaleATARGradesSettingButtonContent { get; set; }

    /// <summary>
    /// The content of the position navigation controls button.
    /// </summary>
    public string NavigationControlsPositionButtonContent { get; set; }

    #endregion

    #region Public Commands

    /// <summary>
    /// The command for when a two-state setting is pressed, to invert its state.
    /// </summary>
    public ICommand InvertSettingCommand { get; set; }

    #endregion 

    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public SettingsPopViewModel()
    {
        // Set the settings buttons' initial content
        UpdateSettingButtonContent();

        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());
        InvertSettingCommand = new RelayParameterizedCommand((parameter) => InvertSetting(parameter));
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Inverts the setting based on the passed setting ID.
    /// </summary>
    /// <param name="parameter">The button's setting's unique ID</param>
    private void InvertSetting(object parameter)
    {
        // Determine the passed setting from the passed setting ID
        ApplicationSetting setting = (ApplicationSetting)Convert.ToInt16(parameter);

        switch (setting)
        {
            case ApplicationSetting.AutoDeleteMessages:
                // Invert the setting
                Settings.Default.AutoDeleteMessages = !Settings.Default.AutoDeleteMessages;
                break;

            case ApplicationSetting.ScaleATARGrades:
                // Invert the setting
                Settings.Default.ScaleATARGrades = !Settings.Default.ScaleATARGrades;
                break;

            case ApplicationSetting.NavigationControlsPosition:
                // Invert the setting
                Settings.Default.ReverseNavigationPosition = !Settings.Default.ReverseNavigationPosition;
                break;

            case ApplicationSetting.DownloadLocation:
                break;
        }

        // Save all settings
        Settings.Default.Save();

        // Update all button content
        UpdateSettingButtonContent();
    }

    /// <summary>
    /// Updates the content of all settings buttons based on the user's current settings.
    /// </summary>
    private void UpdateSettingButtonContent()
    {
        // Update the two boolean buttons' content using an On/Off configuration
        AutoDeleteMessagesSettingButtonContent = Settings.Default.AutoDeleteMessages ? "On" : "Off";
        ScaleATARGradesSettingButtonContent = Settings.Default.ScaleATARGrades ? "On" : "Off";
        NavigationControlsPositionButtonContent = Settings.Default.ReverseNavigationPosition ? "Top-Right" : "Top-Left";
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
