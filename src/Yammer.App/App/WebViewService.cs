//---------------------------------------------------------------------
// <copyright file="WebViewService.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Web view service class
    /// </summary>
    public class WebViewService
    {
        /// <summary>
        /// The HTML property
        /// </summary>
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached("Html", typeof(string), typeof(WebViewService), new PropertyMetadata(null));

        /// <summary>
        /// The size to content property
        /// </summary>
        public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.RegisterAttached("SizeToContent", typeof(bool), typeof(WebViewService), new PropertyMetadata(false));

        /// <summary>
        /// Prevents a default instance of the <see cref="WebViewService"/> class from being created.
        /// </summary>
        private WebViewService()
        {
        }

        /// <summary>
        /// Sets the HTML.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="html">The HTML.</param>
        public static void SetHtml(DependencyObject element, string html)
        {
            string oldHtml = WebViewService.GetHtml(element);
            element.SetValue(WebViewService.HtmlProperty, html);
            if (string.IsNullOrEmpty(oldHtml) && !string.IsNullOrEmpty(html))
            {
                WebView view = (WebView)element.ThrowIfNull(nameof(element));
                view.Loaded += (s, e) => view.NavigateToString(html);
            }
        }

        /// <summary>
        /// Gets the HTML.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The HTML.
        /// </returns>
        public static string GetHtml(DependencyObject element)
        {
            return (string)element.ThrowIfNull(nameof(element)).GetValue(WebViewService.HtmlProperty);
        }

        /// <summary>
        /// Sets the content of the size to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetSizeToContent(DependencyObject element, bool value)
        {
            bool oldValue = WebViewService.GetSizeToContent(element);
            element.ThrowIfNull(nameof(element)).SetValue(WebViewService.SizeToContentProperty, value);

            if(!oldValue && value)
            {
                WebView view = (WebView)element;
                view.NavigationCompleted += async (s, e) =>
                {
                    if (e.IsSuccess)
                    {
                        string heightString = await view.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
                        int height;
                        if (int.TryParse(heightString, out height))
                        {
                            view.Height = height;
                        }

                        string widthString = await view.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
                        int width;
                        if (int.TryParse(widthString, out width))
                        {
                            view.Width = width;
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Gets the content of the size to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static bool GetSizeToContent(DependencyObject element)
        {
            return (bool)element.GetValue(WebViewService.SizeToContentProperty);
        }
    }
}
