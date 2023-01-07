using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace SACEology.Animation
{
    /// <summary>
    /// Helpers to animate elements in specific ways.
    /// </summary>
    public static class PageAnimations
    {
        /// <summary>
        /// Slide the element in from the left.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task SlideAndFadeInFromLeft(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add slide animation
            sb.AddSlideFromLeft(seconds, 1000);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Begin the animations
            sb.Begin(element);

            // Make page visible one everything is set up
            element.Visibility = Visibility.Visible;

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slide the element out to the right.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task SlideAndFadeOutToRight(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add slide animation
            sb.AddSlideToRight(seconds, 1000);

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Begin the animations
            sb.Begin(element);

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));

            // Remove this element
            element.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Slide the element in from the bottom.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task SlideAndFadeInFromBottom(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add slide animation
            sb.AddSlideFromBottom(seconds, 1000);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Begin the animations
            sb.Begin(element);

            // Make page visible one everything is set up
            element.Visibility = Visibility.Visible;

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slide the element out to the right.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task SlideAndFadeOutToTop(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add slide animation
            sb.AddSlideToTop(seconds, 1000);

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Begin the animations
            sb.Begin(element);

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));

            // Remove this element
            element.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Fades an element out.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task StaticFadeIn(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Begin the animations
            sb.Begin(element);

            // Remove this element
            element.Visibility = Visibility.Visible;

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Fades an element out.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task StaticFadeOut(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Begin the animations
            sb.Begin(element);

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));

            // Remove this element
            element.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Fades an element out.
        /// </summary>
        /// <param name="page">The element</param>
        /// <param name="seconds">The animation duration</param>
        /// <returns>Returns that the task is complete</returns>
        public static async Task StaticFadeInThenOut(this FrameworkElement element, float seconds)
        {
            // Set up new storyboard
            var sb = new Storyboard();

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Begin the animations
            sb.Begin(element);

            // Remove this element
            element.Visibility = Visibility.Visible;

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000 * 10));

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Begin the animations
            sb.Begin(element);

            // Wait for this task to finish
            await Task.Delay((int)(seconds * 1000));

            // Remove this element
            element.Visibility = Visibility.Collapsed;
        }
    }
}
