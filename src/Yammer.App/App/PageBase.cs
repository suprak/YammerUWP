//---------------------------------------------------------------------
// <copyright file="PageBase.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Page base class
    /// </summary>
    public class PageBase : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PageBase), new PropertyMetadata("##TITLE##"));

        /// <summary>
        /// The when navigated to timestamp
        /// </summary>
        private DateTimeOffset whenNavigatedTo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageBase"/> class.
        /// </summary>
        public PageBase()
        {
            // Necessary so that parent Frame's context does not automatically transfer
            // might have to make sure behavior overridable
            this.DataContext = null;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                return (string)this.GetValue(PageBase.TitleProperty);
            }

            set
            {
                this.SetValue(PageBase.TitleProperty, value);
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        protected App App
        {
            get { return App.Instance; }
        }

        /// <summary>
        /// Tries to re-navigate to this page.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// true if successfull, false otherwise
        /// </returns>
        public virtual bool TryReNavigateTo(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.whenNavigatedTo = DateTimeOffset.Now;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedFrom" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            TelemetryClient client = new TelemetryClient();
            PageViewTelemetry telemetry = new PageViewTelemetry(this.GetType().FullName)
            {
                Duration = DateTimeOffset.Now - this.whenNavigatedTo,
            };

            client.TrackPageView(telemetry);
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

#if false
        /// <summary>
        /// Dropws the shadow_ draw.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs" /> instance containing the event data.</param>
        protected void DropShadow_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Color[] colors = new Color[(int)(sender.Size.Width * sender.Size.Height)];
            for (int x = 0; x < sender.ActualWidth; x++)
            {
                if (x < sender.ActualWidth)
                {
                    colors[x] = Colors.{ThemeResource SystemControlForegroundChromeHighBrush};
                }
            }

            using (CanvasBitmap bitmap = CanvasBitmap.CreateFromColors(sender, colors, (int)sender.Size.Width, (int)sender.Size.Height))
            {
                using (ShadowEffect shadow = new ShadowEffect())
                {
                    shadow.Source = bitmap;
                    shadow.BlurAmount = 3;
                    shadow.Optimization = EffectOptimization.Speed;

                    args.DrawingSession.DrawImage(shadow);
                }
            }
        }
#endif
    }
}
