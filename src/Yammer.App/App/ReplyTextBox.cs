//---------------------------------------------------------------------
// <copyright file="ReplyTextBox.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Reply text box class
    /// </summary>
    public class ReplyTextBox : TextBox
    {
        /// <summary>
        /// The has text property
        /// </summary>
        public static readonly DependencyProperty HasTextProperty = DependencyProperty.Register("HasText", typeof(bool), typeof(ReplyTextBox), new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyTextBox"/> class.
        /// </summary>
        public ReplyTextBox()
        {
            this.TextChanged += (s, e) => { this.HasText = !string.IsNullOrEmpty(this.Text); };
        }

        /// <summary>
        /// Gets a value indicating whether this instance has text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has text; otherwise, <c>false</c>.
        /// </value>
        public bool HasText
        {
            get { return (bool)this.GetValue(ReplyTextBox.HasTextProperty); }
            private set { this.SetValue(ReplyTextBox.HasTextProperty, value); }
        }
    }
}
