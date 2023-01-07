using SACEology.Animation;

namespace SACEology
{
    /// <summary>
    /// Interaction logic for MainMenuPage.xaml
    /// </summary>
    public partial class ErrorPopUp : BasePopUp
    {
        public ErrorPopUp(string message)
        {
            InitializeComponent();
            PopUpLoadAnimation = StoryboardAnimation.FadeInThenOutStatic;

            Message.Text = message;
        }
    }
}
