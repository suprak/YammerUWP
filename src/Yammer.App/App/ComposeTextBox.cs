//---------------------------------------------------------------------
// <copyright file="ComposeTextBox.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// Compose text box class
    /// </summary>
    public class ComposeTextBox : Control
    {
        /// <summary>
        /// The placeholder text property
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(ComposeTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// The separator alignment property
        /// </summary>
        public static readonly DependencyProperty SeparatorAlignmentProperty = DependencyProperty.Register("SeparatorAlignment", typeof(SeparatorAlignment), typeof(ComposeTextBox), new PropertyMetadata(SeparatorAlignment.None));

        /// <summary>
        /// The header property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(ComposeTextBox), new PropertyMetadata(null));

        /// <summary>
        /// The text property
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ComposeTextBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposeTextBox"/> class.
        /// </summary>
        public ComposeTextBox()
        {
            this.DefaultStyleKey = typeof(ComposeTextBox);
        }

        /// <summary>
        /// Occurs when the send button is tapped.
        /// </summary>
        public event TappedEventHandler SendTapped;

        /// <summary>
        /// Occurs when the attach button is tapped.
        /// </summary>
        public event TappedEventHandler AttachTapped;

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        /// <value>
        /// The placeholder text.
        /// </value>
        public string PlaceholderText
        {
            get { return (string)this.GetValue(ComposeTextBox.PlaceholderTextProperty); }
            set { this.SetValue(ComposeTextBox.PlaceholderTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the separator alignment.
        /// </summary>
        /// <value>
        /// The separator alignment.
        /// </value>
        public SeparatorAlignment SeparatorAlignment
        {
            get { return (SeparatorAlignment)this.GetValue(ComposeTextBox.SeparatorAlignmentProperty); }
            set { this.SetValue(ComposeTextBox.SeparatorAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public object Header
        {
            get { return this.GetValue(ComposeTextBox.HeaderProperty); }
            set { this.SetValue(ComposeTextBox.HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get { return (string)this.GetValue(ComposeTextBox.TextProperty); }
            set { this.SetValue(ComposeTextBox.TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text box.
        /// </summary>
        /// <value>
        /// The text box.
        /// </value>
        private TextBox TextBox
        {
            get;
            set;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest terms, this means the method is called just before a UI element displays in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.SeparatorAlignment == SeparatorAlignment.Top)
            {
                this.GetTemplateChild("PART_TopSeparator");
            }
            else if (this.SeparatorAlignment == SeparatorAlignment.Bottom)
            {
                this.GetTemplateChild("PART_BottomSeparator");
            }

            Button sendButton = (Button)this.GetTemplateChild("PART_SendButton");
            sendButton.Tapped += (s, e) => this.SendTapped?.Invoke(this, e);

            Button attachButton = (Button)this.GetTemplateChild("PART_AttachButton");
            attachButton.Tapped += (s, e) => this.AttachTapped?.Invoke(this, e);

            this.TextBox = (TextBox)this.GetTemplateChild("PART_TextBox");
            this.TextBox.TextChanged += (s, e) => sendButton.IsEnabled = !string.IsNullOrEmpty(this.TextBox.Text);
        }

        /// <summary>
        /// Focuses the text.
        /// </summary>
        public void FocusText()
        {
            this.TextBox?.Focus(FocusState.Programmatic);
        }
    }

    /// <summary>
    /// Separator alignment enum
    /// </summary>
    public enum SeparatorAlignment
    {
        None,
        Top,
        Bottom
    }
}
