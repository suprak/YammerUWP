//---------------------------------------------------------------------
// <copyright file="SwipeableControl.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using Windows.Devices.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// Swipeble control class
    /// </summary>
    public class SwipeableControl : ContentControl
    {
        /// <summary>
        /// The left content property
        /// </summary>
        public static readonly DependencyProperty LeftContentProperty = DependencyProperty.Register("LeftContent", typeof(object), typeof(SwipeableControl), new PropertyMetadata(null));

        /// <summary>
        /// The left template property
        /// </summary>
        public static readonly DependencyProperty LeftTemplateProperty = DependencyProperty.Register("LeftTemplate", typeof(DataTemplate), typeof(SwipeableControl), new PropertyMetadata(null));

        /// <summary>
        /// The right content property
        /// </summary>
        public static readonly DependencyProperty RightContentProperty = DependencyProperty.Register("RightContent", typeof(object), typeof(SwipeableControl), new PropertyMetadata(null));

        /// <summary>
        /// The right template property
        /// </summary>
        public static readonly DependencyProperty RightTemplateProperty = DependencyProperty.Register("RightTemplate", typeof(DataTemplate), typeof(SwipeableControl), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SwipeableControl"/> class.
        /// </summary>
        public SwipeableControl()
        {
            this.DefaultStyleKey = typeof(SwipeableControl);
        }

        /// <summary>
        /// Occurs when the user swipes left.
        /// </summary>
        public event EventHandler SwipedLeft;

        /// <summary>
        /// Occurs when the user swipes right.
        /// </summary>
        public event EventHandler SwipedRight;

        /// <summary>
        /// Gets or sets the content of the left.
        /// </summary>
        /// <value>
        /// The content of the left.
        /// </value>
        public object LeftContent
        {
            get { return this.GetValue(SwipeableControl.LeftContentProperty); }
            set { this.SetValue(SwipeableControl.LeftContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the left template.
        /// </summary>
        /// <value>
        /// The left template.
        /// </value>
        public DataTemplate LeftTemplate
        {
            get { return (DataTemplate)this.GetValue(SwipeableControl.LeftTemplateProperty); }
            set { this.SetValue(SwipeableControl.LeftTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content of the right.
        /// </summary>
        /// <value>
        /// The content of the right.
        /// </value>
        public object RightContent
        {
            get { return this.GetValue(SwipeableControl.RightContentProperty); }
            set { this.SetValue(SwipeableControl.RightContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the right template.
        /// </summary>
        /// <value>
        /// The right template.
        /// </value>
        public DataTemplate RightTemplate
        {
            get { return (DataTemplate)this.GetValue(SwipeableControl.RightTemplateProperty); }
            set { this.SetValue(SwipeableControl.RightTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the left presenter.
        /// </summary>
        /// <value>
        /// The left presenter.
        /// </value>
        private ContentControl LeftPresenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the right presenter.
        /// </summary>
        /// <value>
        /// The right presenter.
        /// </value>
        private ContentControl RightPresenter
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the left presenter visiblity.
        /// </summary>
        /// <param name="targetVisibility">The target visibility.</param>
        private void SetLeftPresenterVisiblity(Visibility targetVisibility)
        {
            if (this.LeftPresenter == null)
            {
                this.LeftPresenter = (ContentControl)this.GetTemplateChild("PART_LeftContent");
            }

            this.LeftPresenter.Visibility = targetVisibility;
        }

        /// <summary>
        /// Sets the right presenter visibility.
        /// </summary>
        /// <param name="targetVisibility">The target visibility.</param>
        private void SetRightPresenterVisibility(Visibility targetVisibility)
        {
            if (this.RightPresenter == null)
            {
                this.RightPresenter = (ContentControl)this.GetTemplateChild("PART_RightContent");
            }

            this.RightPresenter.Visibility = targetVisibility;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest terms, this means the method is called just before a UI element displays in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Grid root = (Grid)this.GetTemplateChild("PART_Root");

            ContentPresenter content = (ContentPresenter)this.GetTemplateChild("PART_Content");
            TranslateTransform transform = (TranslateTransform)this.GetTemplateChild("PART_ContentTransform");

            Storyboard restoreAnimation = (Storyboard)this.GetTemplateChild("STORY_RestoreSwipe");
            Storyboard expandLeftAnimation = (Storyboard)this.GetTemplateChild("STORY_ExpandLeft");
            Storyboard expandRightAnimation = (Storyboard)this.GetTemplateChild("STORY_ExpandRight");
            
            expandLeftAnimation.Completed += (ss, ee) => restoreAnimation.Begin();
            expandRightAnimation.Completed += (ss, ee) => restoreAnimation.Begin();

            if (content != null && transform != null)
            {
                content.ManipulationDelta += (s, e) =>
                {
                    if (e.PointerDeviceType == PointerDeviceType.Touch)
                    {
                        if (Math.Abs(e.Cumulative.Translation.X) == e.Cumulative.Translation.X)
                        {
                            this.SetLeftPresenterVisiblity(Visibility.Visible);
                            this.SetRightPresenterVisibility(Visibility.Collapsed);
                        }
                        else
                        {
                            this.SetRightPresenterVisibility(Visibility.Visible);
                            this.SetLeftPresenterVisiblity(Visibility.Collapsed);
                        }

                        transform.X = e.Cumulative.Translation.X;
                    }
                };

                content.ManipulationCompleted += (s, e) =>
                {
                    if (e.PointerDeviceType == PointerDeviceType.Touch)
                    {
                        ((DoubleAnimation)expandLeftAnimation.Children[0]).To = -content.ActualWidth;
                        ((DoubleAnimation)expandRightAnimation.Children[0]).To = content.ActualWidth;

                        double translation = Math.Abs(e.Cumulative.Translation.X);
                        if (translation / this.ActualWidth >= 0.25)
                        {
                            if (translation == e.Cumulative.Translation.X)
                            {
                                // right swipe
                                expandRightAnimation.Begin();
                                this.SwipedRight?.Invoke(this, new EventArgs());
                            }
                            else
                            {
                                // left swipe
                                expandLeftAnimation.Begin();
                                this.SwipedLeft?.Invoke(this, new EventArgs());
                            }
                        }
                        else
                        {
                            restoreAnimation.Begin();
                        }
                    }
                };
            }
        }
    }
}
