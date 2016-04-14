//---------------------------------------------------------------------
// <copyright file="App.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Windows.ApplicationModel.Activation;
    using Windows.Foundation;
    using Windows.System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// App class
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            // TODO: Enable if you want to use application insights
            //WindowsAppInitializer.InitializeAsync(
            //        "YOUR_KEY_HERE",
            //        WindowsCollectors.Metadata |
            //        WindowsCollectors.Session |
            //        WindowsCollectors.UnhandledException);
            // TODO: Remove if you enable telemetry
            TelemetryConfiguration.Active.DisableTelemetry = true;
#if DEBUG
            TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
#endif

            this.InitializeComponent();
            Logger.Instance = new TelemetryLogger();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static App Instance
        {
            get { return (App)App.Current; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is narrow screen.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is narrow screen; otherwise, <c>false</c>.
        /// </value>
        public bool IsNarrowScreen
        {
            get { return Window.Current.Bounds.Width < 720; }
        }

        /// <summary>
        /// Gets or sets the root frame.
        /// </summary>
        /// <value>
        /// The root frame.
        /// </value>
        private Frame RootFrame
        {
            get;
            set;
        }

        /// <summary>
        /// Navigates to github, asynchronously.
        /// </summary>
        /// <returns>
        /// The completion task.
        /// </returns>
        public static IAsyncOperation<bool> NavigateToGithubAsync()
        {
            return Launcher.LaunchUriAsync(new Uri("https://github.com/suprak/YammerUWP"));
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            this.Launch();
            this.RootFrame.BackStack.Clear();
            this.RootFrame.ForwardStack.Clear();
            this.RootFrame.Navigate(typeof(AuthPage), new AuthPage.Arguments(e.Arguments));
            Window.Current.Activate();
        }

        /// <summary>
        /// Raises the <see cref="E:Activated" /> event.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> instance containing the event data.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            this.Launch();
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = (ProtocolActivatedEventArgs)args;
                this.RootFrame.BackStack.Clear();
                this.RootFrame.ForwardStack.Clear();
                ProtocolLaunchArguments arguments = new ProtocolLaunchArguments(eventArgs.Uri);
                if (arguments.Type.Equals("Auth", StringComparison.OrdinalIgnoreCase))
                {
                    this.RootFrame.Navigate(typeof(AuthPage), new AuthPage.Arguments(arguments.Uri));
                }
            }

            Window.Current.Activate();
        }

        /// <summary>
        /// Launches this instance.
        /// </summary>
        private void Launch()
        {
            this.RootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (this.RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                this.RootFrame = new Frame();

                this.RootFrame.NavigationFailed += this.OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = this.RootFrame;
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Protocol launch arguments class
        /// </summary>
        private class ProtocolLaunchArguments
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ProtocolLaunchArguments"/> class.
            /// </summary>
            /// <param name="uri">The URI.</param>
            public ProtocolLaunchArguments(Uri uri)
            {
                this.Uri = uri.ThrowIfNull(nameof(uri));
                this.Type = uri.Host;
            }

            /// <summary>
            /// Gets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public string Type
            {
                get;
            }

            /// <summary>
            /// Gets the URI.
            /// </summary>
            /// <value>
            /// The URI.
            /// </value>
            public Uri Uri
            {
                get;
            }
        }
    }
}
