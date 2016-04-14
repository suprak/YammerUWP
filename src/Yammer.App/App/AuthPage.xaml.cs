//---------------------------------------------------------------------
// <copyright file="AuthPage.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using API;
    using Windows.System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Auth page class
    /// </summary>
    public sealed partial class AuthPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthPage"/> class.
        /// </summary>
        public AuthPage()
        {
            this.InitializeComponent();
            this.Loaded += this.AuthPage_Loaded;
        }

        /// <summary>
        /// Gets the page arguments.
        /// </summary>
        /// <value>
        /// The page arguments.
        /// </value>
        public Arguments PageArguments
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.PageArguments = (Arguments)e.Parameter;
        }

        /// <summary>
        /// Handles the Loaded event of the AuthPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AuthPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Frame.BackStack.Clear();

            OAuthToken token = null;
            if (this.PageArguments.State == Arguments.States.UnAuthenticated ||
                (this.PageArguments.State == Arguments.States.Authenticated &&
                !OAuthToken.TryGetValue(out token)))
            {
                this.LaunchBrowser();
            }
            else if (this.PageArguments.State == Arguments.States.Authenticating)
            {
                token = OAuthClient.Instance.GetAccessToken(this.PageArguments.AutheticationPayload);
                if (token == null && !OAuthToken.TryGetValue(out token))
                {
                    throw new InvalidOperationException("Unable to authenticate with the provided token payload.");
                }
            }

            if (token != null)
            {
                this.Frame.Navigate(typeof(MainPage), new MainPage.Arguments(this.PageArguments.LaunchArguments));
            }
        }

        /// <summary>
        /// Handles the Click event of the Retry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchBrowser();
        }

        /// <summary>
        /// Launches the browser.
        /// </summary>
        private void LaunchBrowser()
        {
            this.RetryLink.Visibility = Visibility.Visible;
            var t = Launcher.LaunchUriAsync(YammerService.ClientInfo.TargetUri);
        }

        /// <summary>
        /// Arguments class
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Arguments" /> class.
            /// </summary>
            /// <param name="state">The state.</param>
            /// <param name="launchArguments">The launch arguments.</param>
            public Arguments(string launchArguments)
            {
                this.State = States.Authenticated;
                this.LaunchArguments = launchArguments;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Arguments"/> class.
            /// </summary>
            /// <param name="payload">The payload.</param>
            public Arguments(Uri payload)
            {
                this.State = States.Authenticating;
                this.AutheticationPayload = payload.ThrowIfNull(nameof(payload));
                this.LaunchArguments = string.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Arguments"/> class.
            /// </summary>
            public Arguments()
            {
                this.State = States.UnAuthenticated;
            }

            /// <summary>
            /// States enum
            /// </summary>
            public enum States
            {
                UnAuthenticated,
                Authenticating,
                Authenticated,
            }

            /// <summary>
            /// Gets the state.
            /// </summary>
            /// <value>
            /// The state.
            /// </value>
            public States State
            {
                get;
            }

            /// <summary>
            /// Gets the launch arguments.
            /// </summary>
            /// <value>
            /// The launch arguments.
            /// </value>
            public string LaunchArguments
            {
                get;
            }

            /// <summary>
            /// Gets the authetication payload.
            /// </summary>
            /// <value>
            /// The authetication payload.
            /// </value>
            public Uri AutheticationPayload
            {
                get;
            }
        }
    }
}
