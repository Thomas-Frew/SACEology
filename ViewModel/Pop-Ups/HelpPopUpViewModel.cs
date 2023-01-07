using SACEology.ViewModel.Base;
using System.Windows.Input;
using SACEology;

class HelpPopUpViewModel : BaseViewModel
{
    #region Constructor

    /// <summary>
    /// The default constructor.
    /// </summary>
    /// <param name="window"></param>
    public HelpPopUpViewModel()
    {
        ClosePopUpCommand = new RelayCommand(() => ClosePopUp());
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
