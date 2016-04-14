//---------------------------------------------------------------------
// <copyright file="GroupPage.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System.Threading.Tasks;
    using API;
    using API.Models;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Group page class
    /// </summary>
    public sealed partial class GroupPage : MessagesPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPage"/> class.
        /// </summary>
        public GroupPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public Group Group
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatedTo" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Group = (Group)e.Parameter;
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Gets the page identifier.
        /// </summary>
        /// <returns>
        /// The page identifier.
        /// </returns>
        protected override int GetPageId()
        {
            return this.Group.Id;
        }

        /// <summary>
        /// Creates the model, asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The created model task.
        /// </returns>
        protected override Task<MessagesViewModel> CreateModelAsync(int id)
        {
            return MessagesViewModel.CreateAsync(id, this.GetGroupMessagesAsync, this.Group.Id);
        }

        /// <summary>
        /// Called when the model is updated.
        /// </summary>
        protected override void OnModelUpdated()
        {
            this.Bindings.Update();
        }

        /// <summary>
        /// Tries to re-navigate to this page.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// true if successfull, false otherwise
        /// </returns>
        public override bool TryReNavigateTo(object parameter)
        {
            Group group = (Group)parameter;
            return this.Group.Id == group.Id;
        }

        /// <summary>
        /// Gets the group messages, asynchronously.
        /// </summary>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The group messages.
        /// </returns>
        private Task<MessageFeed> GetGroupMessagesAsync(int olderThanId = -1, int newerThanId = -1, int limit = -1)
        {
            return YammerService.Instance.GetGroupMessagesAsync(this.Group.Id, olderThanId, newerThanId, limit);
        }

        /// <summary>
        /// Handles the Tapped event of the PostUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TappedRoutedEventArgs"/> instance containing the event data.</param>
        private void PostUpdate_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ListView.ScrollIntoView(this.ListView.Header);
            this.ComposeTextBox.FocusText();
        }

        /// <summary>
        /// Handles the Tapped event of the PostMessage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TappedRoutedEventArgs"/> instance containing the event data.</param>
        private void PostMessage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Task task = this.Model.PostMessageAsync();
        }
    }
}
