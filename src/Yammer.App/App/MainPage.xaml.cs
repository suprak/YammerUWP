//---------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using API;
    using Windows.Foundation.Metadata;
    using Windows.Phone.UI.Input;
    using Windows.Storage;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using Yammer.API.Models;

    /// <summary>
    /// Main page class
    /// </summary>
    public sealed partial class MainPage : PageBase
    {
        /// <summary>
        /// The refresh timer
        /// </summary>
        private readonly DispatcherTimer refreshTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            if (ApiInformation.IsTypePresent(typeof(SystemNavigationManager).FullName))
            {
                SystemNavigationManager navigationManager = SystemNavigationManager.GetForCurrentView();
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                navigationManager.BackRequested += (s, ee) => ee.Handled = this.NavigateBack();
            }

            if (ApiInformation.IsTypePresent(typeof(HardwareButtons).FullName))
            {
                HardwareButtons.BackPressed += (s, ee) => ee.Handled = this.NavigateBack();
            }

            this.refreshTimer = new DispatcherTimer();
            this.refreshTimer.Tick += (s, e) =>
            {
                // only refresh groups etc if pane is open
                // otherwise we will refresh when user opens it
                if (this.SplitView.IsPaneOpen)
                {
                    this.Model?.Refresh(true);
                }
            };
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
        /// Navigates to a new sub page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="refresh">if set to <c>true</c> [refresh].</param>
        public void NavigateTo(Type pageType, object parameter = null, bool force = false)
        {
            if (this.MainFrame != null)
            {
                this.PostPaneAction();
                if (this.MainFrame.CurrentSourcePageType != pageType || force)
                {
                    this.MainFrame.Navigate(pageType, parameter);
                }
                else
                {
                    PageBase page = (PageBase)this.MainFrame.Content;
                    if (page.TryReNavigateTo(parameter))
                    {
                        this.RefreshCurrentPage(false);
                    }
                    else
                    {
                        this.MainFrame.Navigate(pageType, parameter);
                    }
                }
            }
        }

        /// <summary>
        /// Shows the image attachment, asynchronously.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        public async Task ShowImageAttachmentAsync(ImageAttachment target)
        {
            target.ThrowIfNull(nameof(target));

            bool error = false;
            try
            {
                IStorageFile file = await target.LoadAsync();
                LauncherOptions options = new LauncherOptions();
                options.DesiredRemainingView = ViewSizePreference.UseMore;
                options.TargetApplicationPackageFamilyName = "Microsoft.Windows.Photos_8wekyb3d8bbwe";
                bool result = await Launcher.LaunchFileAsync(file, options);
                if (!result)
                {
                    options = new LauncherOptions();
                    options.DisplayApplicationPicker = true;
                    error = !await Launcher.LaunchFileAsync(file, options);
                }
            }
            catch (Exception exception)
            {
                error = true;
                Logger.Instance.LogException(exception);
            }

            if (error)
            {
                MessageDialog dialog = new MessageDialog("Unable to open attachment.", "Attachment Error");
                var dialogTask = dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Arguments arguments = (Arguments)e.Parameter;

            this.Frame.ClearHistory();

            if (this.Model == null)
            {
                this.Model = await ViewModel.CreateAsync();
                this.refreshTimer.Interval = this.Model.RefreshInterval;
            }

            this.refreshTimer.Start();
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedFrom" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.refreshTimer.Stop();
        }

        /// <summary>
        /// Handles the Click event of the Hamburger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            this.Model?.Refresh();
            this.SplitView.IsPaneOpen = !this.SplitView.IsPaneOpen;
        }

        /// <summary>
        /// Navigates the main frame back.
        /// </summary>
        /// <returns>
        /// true if can go back, false otherwise
        /// </returns>
        private bool NavigateBack()
        {
            bool result = false;
            if (this.MainFrame != null && this.MainFrame.CanGoBack)
            {
                this.MainFrame.GoBack();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Refreshes the current page.
        /// </summary>
        private void RefreshCurrentPage(bool forceRefresh = true)
        {
            PageBase page = this.MainFrame.Content as PageBase;
            if (page != null)
            {
                ViewModelBase model = page.DataContext as ViewModelBase;
                model?.Refresh(forceRefresh);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the MessageAreas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void MessageAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView list = (ListView)sender;
            switch (list.SelectedIndex)
            {
                case 0:
                    this.NavigateTo(typeof(MyFeedPage));
                    break;
                case 1:
                    this.NavigateTo(typeof(InboxPage));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Groups control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView list = (ListView)sender;
            Group group = (Group)list.SelectedItem;
            this.NavigateTo(typeof(GroupPage), group, false);

            this.GroupsListView.SelectionChanged -= this.Groups_SelectionChanged;
            this.GroupsListView.SelectedIndex = -1;
            this.GroupsListView.SelectionChanged += this.Groups_SelectionChanged;
        }

        /// <summary>
        /// Handles the Click event of the Home control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateTo(typeof(MyFeedPage));
        }

        /// <summary>
        /// Handles the Click event of the Inbox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void Inbox_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateTo(typeof(InboxPage));
        }

        /// <summary>
        /// Handles the Click event of the Notifications control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Posts the pane action.
        /// </summary>
        private void PostPaneAction()
        {
            if (this.SplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                this.SplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the Settings control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateTo(typeof(SettingsPage));
        }

        /// <summary>
        /// Handles the Changed event of the ActiveNetwork control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void ActiveNetwork_Changed(object sender, SelectionChangedEventArgs e)
        {
            // don't force a refresh if this is the first selection
            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
            {
                Network network = (Network)e.AddedItems.FirstOrDefault();
                if (network != null)
                {
                    this.Model.Network = network;
                    YammerService.Instance.SetActiveNetwork(this.Model.Network);
                    this.Model.Refresh(true);
                }

                this.MainFrame.Content = null;
                this.MainFrame.ClearHistory();
                this.NavigateTo(typeof(MyFeedPage), null, true);
                this.MainFrame.ClearHistory();
            }
        }

        /// <summary>
        /// Handles the Click event of the GitHubLink control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void GitHubLink_Click(object sender, RoutedEventArgs e)
        {
            var task = App.NavigateToGithubAsync();
        }
    }
}
