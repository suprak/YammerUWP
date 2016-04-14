//---------------------------------------------------------------------
// <copyright file="TextBlockService.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Text.RegularExpressions;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Text block service class
    /// </summary>
    public class TextBlockService
    {
        /// <summary>
        /// The formatted text property
        /// </summary>
        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.Register(
                "FormattedText",
                typeof(string),
                typeof(TextBlockService),
                new PropertyMetadata(string.Empty, (sender, e) =>
                {
                    string text = (string)e.NewValue;
                    if (sender is TextBlock)
                    {
                        TextBlock block = (TextBlock)sender;
                        block.Inlines.Clear();
                        MatchCollection matches = TextBlockService.HyperlinkPattern.Matches(text);
                        int position = 0;
                        foreach (Match match in matches)
                        {
                            if (match.Index - position > 0)
                            {
                                block.Inlines.Add(new Run() { Text = text.Substring(position, match.Index - position) });
                                position = match.Index + match.Length;
                            }

                            bool uriCreated = false;
                            Uri uri;
                            try
                            {
                                if (Uri.TryCreate(match.Value, UriKind.RelativeOrAbsolute, out uri))
                                {
                                    Hyperlink link = new Hyperlink() { NavigateUri = uri, Foreground = (SolidColorBrush)App.Current.Resources["SystemControlHyperlinkTextBrush"] };
                                    link.Inlines.Add(new Run() { Text = match.Value });
                                    block.Inlines.Add(link);
                                    uriCreated = true;
                                }
                            }
                            catch
                            {
                            }

                            if (!uriCreated)
                            {
                                block.Inlines.Add(new Run() { Text = match.Value });
                            }
                        }

                        block.Inlines.Add(new Run() { Text = text.Substring(position) });
                    }
                }));

        /// <summary>
        /// The hyperlink pattern
        /// </summary>
        private static readonly Regex HyperlinkPattern = new Regex(@"(http[s]{0,1}://[^\s<>]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Prevents a default instance of the <see cref="TextBlockService"/> class from being created.
        /// </summary>
        private TextBlockService()
        {
        }

        /// <summary>
        /// Gets the formatted text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The formatted text.
        /// </returns>
        public static string GetFormattedText(DependencyObject element)
        {
            return (string)element.GetValue(FormattedTextProperty);
        }

        /// <summary>
        /// Sets the formatted text.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetFormattedText(DependencyObject element, string value)
        {
            element.SetValue(FormattedTextProperty, value);
        }
    }
}
