//---------------------------------------------------------------------
// <copyright file="PrivateImageService.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Imaging;
    using Yammer.API;

    /// <summary>
    /// Private image service class
    /// </summary>
    public class PrivateImageService
    {
        /// <summary>
        /// The source property
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("SourceProperty", typeof(string), typeof(PrivateImageService), new PropertyMetadata(null, PrivateImageService.OnSourceChanged));

        /// <summary>
        /// Prevents a default instance of the <see cref="PrivateImageService"/> class from being created.
        /// </summary>
        private PrivateImageService()
        {
        }

        /// <summary>
        /// Sets the source.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The URI.</param>
        public static void SetSource(DependencyObject element, string value)
        {
            element.SetValue(PrivateImageService.SourceProperty, value);
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The source value.
        /// </returns>
        public static string GetSource(DependencyObject element)
        {
            return (string)element.GetValue(PrivateImageService.SourceProperty);
        }

        /// <summary>
        /// Called when source changes.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSourceChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            string oldValue = (string)args.OldValue;
            string newValue = (string)args.NewValue;
            if (string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(newValue))
            {
                if (element is Image)
                {
                    Image image = (Image)element;
                    image.Loaded += (s, e) =>
                    {
                        Image internalImage = (Image)s;
                        if (internalImage.Source == null)
                        {
                            string _currentUri = PrivateImageService.GetSource((DependencyObject)s);
                            Task task = PrivateImageService.LoadPrivateSourceAsync(image, _currentUri);
                        }
                    };
                }
                //else if (element is WebView)
                //{
                //    WebView view = (WebView)element;
                //    view.Loaded += async (s, e) =>
                //    {
                //        string _currentUri = PrivateImageService.GetSource((DependencyObject)s);
                //        HttpRequestMessage request = await YammerService.Instance.GetPrivateResourceRequestMessageAsync(_currentUri);
                //        view.NavigateWithHttpRequestMessage(request);
                //    };
                //}
            }
        }

        /// <summary>
        /// Loads the private source, asynchronously.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        private static async Task LoadPrivateSourceAsync(Image image, string uri)
        {
            if (image != null)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (Stream stream = await YammerService.Instance.GetPrivateResourceStreamAsync(uri))
                    using (IRandomAccessStream randomStream = stream.AsRandomAccessStream())
                    {
                        await bitmap.SetSourceAsync(randomStream);
                    }

                    image.Source = bitmap;
                }
                catch (Exception exception)
                {
                    Logger.Instance.LogException(exception);
                }
            }
        }
    }
}
