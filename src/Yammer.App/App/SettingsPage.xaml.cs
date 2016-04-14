//---------------------------------------------------------------------
// <copyright file="SettingsPage.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Threading.Tasks;
    using API;
    using API.Models;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Settings page class
    /// </summary>
    public sealed partial class SettingsPage : PageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public ViewModel Model
        {
            get
            {
                return (ViewModel)this.DataContext;
            }

            set
            {
                this.DataContext = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Model = new ViewModel();

            if(App.RequestedTheme == ApplicationTheme.Light)
            {
                this.LightTheme.IsChecked = true;
            }
            else
            {
                this.DarkTheme.IsChecked = true;
            }

            this.Model.Refresh();
        }

        /// <summary>
        /// Handles the Click event of the Logout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("The application will exit.\nAre you sure you want to log out?", "Log out");
            dialog.Commands.Clear();
            dialog.Commands.Add(new UICommand("Yes", (s) =>
            {
                OAuthToken.Delete();
                App.Exit();
            }));

            dialog.Commands.Add(new UICommand("Cancel"));
            dialog.DefaultCommandIndex = 1;
            dialog.CancelCommandIndex = 1;

            await dialog.ShowAsync();
        }

        /// <summary>
        /// View model class
        /// </summary>
        public class ViewModel : ViewModelBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ViewModel"/> class.
            /// </summary>
            public ViewModel()
            {
            }

            /// <summary>
            /// Gets the current user.
            /// </summary>
            /// <value>
            /// The current user.
            /// </value>
            public User CurrentUser
            {
                get { return this.GetProperty<User>(); }
                private set { this.SetProperty(value); }
            }

            /// <summary>
            /// Called when this instance needs to refresh, asynchronously.
            /// </summary>
            /// <returns>
            /// A completion task.
            /// </returns>
            protected override async Task OnRefreshAsync()
            {
                if (this.CurrentUser == null)
                {
                    this.CurrentUser = await YammerService.Instance.GetCurrentUserAsync();
                }
            }
        }
    }
}
