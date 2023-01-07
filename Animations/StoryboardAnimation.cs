namespace SACEology.Animation
{
    /// <summary>
    /// Styles of animations for appearing/dissapearing.
    /// </summary>
    public enum StoryboardAnimation
    {
        /// <summary>
        /// No animations
        /// </summary>
        None = 0,

        /// <summary>
        /// Fade in from left
        /// </summary>
        SlideAndFadeInFromLeft = 1,

        /// <summary>
        /// Fade out to right
        /// </summary>
        SlideAndFadeOutToRight = 2,

        /// <summary>
        /// Fade in from left
        /// </summary>
        SlideAndFadeInFromBottom = 3,

        /// <summary>
        /// Fade out to right
        /// </summary>
        SlideAndFadeOutToTop = 4,

        /// <summary>
        /// Fade in
        /// </summary>
        FadeInStatic = 5,

        /// <summary>
        /// Fade out
        /// </summary>
        FadeOutStatic = 6,

        /// <summary>
        /// Fade in, then out
        /// </summary>
        FadeInThenOutStatic = 7
    }
}

