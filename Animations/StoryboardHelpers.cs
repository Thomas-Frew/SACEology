using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SACEology.Animation
{
    /// <summary>
    /// Animation helpers from <cref="Storyboard"></cref>.
    /// </summary>
    public static class StoryboardHelpers
    {
        /// <summary>
        /// Adds a slide from left animation to the storyboard.
        /// </summary>
        /// <param name="storyboard">The Storyboard</param>
        /// <param name="seconds">The animation duration</param>
        /// <param name="offset">The distance to the left to start from</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        public static void AddSlideFromLeft(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(-offset, 0, keepMargin ? offset : 0, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a slide to right animation to the storyboard.
        /// </summary>
        /// <param name="storyboard">The Storyboard</param>
        /// <param name="seconds">The animation duration</param>
        /// <param name="offset">The distance to the right to end at</param>
        /// <param name="decelerationRatio">The rate of deceleration</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        public static void AddSlideToRight(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f, bool keepMargin = true)
        {
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(keepMargin ? offset : 0, 0, -offset, 0),
                DecelerationRatio = decelerationRatio
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a fade in animation to the storyboard.
        /// </summary>
        /// <param name="storyboard">The Storyboard</param>
        /// <param name="seconds">The animation duration</param>
        public static void AddFadeIn(this Storyboard storyboard, float seconds)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 0,
                To = 1,
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Adds a fade out animation to the storyboard.
        /// </summary>
        /// <param name="storyboard">The Storyboard</param>
        /// <param name="seconds">The animation duration</param>
        public static void AddFadeOut(this Storyboard storyboard, float seconds)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = 1,
                To = 0,
            };

            // Set the target property name
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            // Add this to the storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Add a slide from bottom animation to the storyboard.
        /// </summary>
        /// <param name="storyboard">The Storyboard</param>
        /// <param name="seconds">The animation duration</param>
        /// <param name="offset">The distance to the right to start from</param>
        /// <param name="decelerationRatio"></param>
        public static void AddSlideFromBottom(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
        {
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0, offset, 0, 0),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            // Set property target
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add animation to storyboard
            storyboard.Children.Add(animation);
        }

        /// <summary>
        /// Add a slide to bottom animation to the storyboard.
        /// </summary>
        /// <param name="storyboard">The Storyboard</param>
        /// <param name="seconds">The animation duration</param>
        /// <param name="offset">The distance to the right to start from</param>
        /// <param name="decelerationRatio"></param>
        public static void AddSlideToTop(this Storyboard storyboard, float seconds, double offset, float decelerationRatio = 0.9f)
        {
            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                From = new Thickness(0),
                To = new Thickness(0, -offset, 0, 0),
                DecelerationRatio = decelerationRatio
            };

            // Set property target
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            // Add animation to storyboard
            storyboard.Children.Add(animation);
        }
    }
}