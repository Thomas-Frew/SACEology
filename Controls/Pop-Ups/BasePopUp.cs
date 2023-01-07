using SACEology.Animation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SACEology
{
    /// <summary>
    /// A base interface for all interface to gain base functionality.
    /// </summary>
    public class BasePopUp : UserControl
    {
        #region Public Properties

        /// <summary>
        /// The animation the play when the page is first loaded.
        /// </summary>
        public StoryboardAnimation PopUpLoadAnimation { get; set; } = StoryboardAnimation.SlideAndFadeInFromBottom;

        /// <summary>
        /// The animation the play when the page is unloaded.
        /// </summary>
        public StoryboardAnimation PopUpUnloadAnimation { get; set; } = StoryboardAnimation.SlideAndFadeOutToTop;

        /// <summary>
        /// The time any slide animation takes to complete.
        /// </summary>
        public float SlideSeconds { get; set; } = 1f;

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor.
        /// </summary>
        public BasePopUp()
        {
            // If we are animating in, hide to begin with
            if (PopUpLoadAnimation != StoryboardAnimation.None)
                Visibility = Visibility.Collapsed;

            // Listen out for the page loading
            Loaded += BaseInterface_Loaded;

            PopUpAggregator.OnDeletion += HidePopUp;
        }

        private async void HidePopUp()
        {
            await AnimateOut();
        }

        #endregion

        #region Animation Load / Unload

        /// <summary>
        /// Once the page is loaded, perform any required animation.
        /// </summary>
        /// <param name="sender">The sending control</param>
        /// <param name="e">The event arguments</param>
        private async void BaseInterface_Loaded(object sender, RoutedEventArgs e)
        {
            // Animate the page in
            await AnimateIn();
        }

        /// <summary>
        /// Animate the interface in.
        /// </summary>
        /// <returns></returns>
        public async Task AnimateIn()
        {
            // Make sure we have something to do
            if (this.PopUpLoadAnimation == StoryboardAnimation.None)
                return;

            switch (this.PopUpLoadAnimation)
            {
                case StoryboardAnimation.SlideAndFadeInFromBottom:
                    // Start the animation
                    await this.SlideAndFadeInFromBottom(this.SlideSeconds);
                    break;

                case StoryboardAnimation.FadeInThenOutStatic:
                    // Start the animation
                    await this.StaticFadeInThenOut(this.SlideSeconds);
                    break;
            }

        }

        /// <summary>
        /// Animate the interface out.
        /// </summary>
        /// <returns></returns>
        public async Task AnimateOut()
        {
            // Make sure we have something to do
            if (this.PopUpUnloadAnimation == StoryboardAnimation.None)
                return;

            switch (this.PopUpUnloadAnimation)
            {
                case StoryboardAnimation.SlideAndFadeOutToTop:
                    // Start the animation
                    await this.SlideAndFadeOutToTop(this.SlideSeconds);
                    break;
            }
        }

        #endregion
    }
}
