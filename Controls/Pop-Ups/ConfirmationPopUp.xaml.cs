using SACEology.Animation;
using SACEology.ViewModel;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class ConfirmationPopUp : BasePopUp
    {
        public ConfirmationPopUp(string message)
        {
            InitializeComponent();
            PopUpLoadAnimation = StoryboardAnimation.FadeInThenOutStatic;

            Message.Text = message;
        }
    }
}
