//---------------------------------------------------------------------
// <copyright file="InboxPage.xaml.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System.Threading.Tasks;
    using API;

    /// <summary>
    /// Inbox page class
    /// </summary>
    public sealed partial class InboxPage : MessagesPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InboxPage"/> class.
        /// </summary>
        public InboxPage()
        {
            this.InitializeComponent();
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
            return MessagesViewModel.CreateAsync(id, YammerService.Instance.GetReceivedMessagesAsync);
        }

        /// <summary>
        /// Called when the model is updated.
        /// </summary>
        protected override void OnModelUpdated()
        {
            this.Bindings.Update();
        }
    }
}
