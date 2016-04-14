//---------------------------------------------------------------------
// <copyright file="MessagesViewModel.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using API;
    using API.Models;

    /// <summary>
    /// Messages view model class
    /// </summary>
    public sealed class MessagesViewModel : ViewModelBase, IPostModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesViewModel" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="currentUser">The current user.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="groupId">The group identifier.</param>
        private MessagesViewModel(int id, User currentUser, MessageCollection.MessageFunc messageFunc, int? groupId = null)
            : base(TimeSpan.FromMinutes(5))
        {
            this.Id = id;
            this.CurrentUser = currentUser.ThrowIfNull(nameof(currentUser));
            this.MessageCollection = new MessageCollection(messageFunc.ThrowIfNull(nameof(messageFunc)));
            this.GroupId = groupId;
            this.LastRefresh = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id
        {
            get;
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <value>
        /// The current user.
        /// </value>
        public User CurrentUser
        {
            get;
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public IList<Message> Messages
        {
            get { return this.MessageCollection; }
        }

        /// <summary>
        /// Gets the message collection.
        /// </summary>
        /// <value>
        /// The message collection.
        /// </value>
        public MessageCollection MessageCollection
        {
            get;
        }

        /// <summary>
        /// Gets or sets the reply message.
        /// </summary>
        /// <value>
        /// The reply message.
        /// </value>
        public string ReplyMessage
        {
            get { return this.GetProperty<string>(); }
            set { this.SetProperty(value); }
        }

        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        public int? GroupId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the reply to.
        /// </summary>
        /// <value>
        /// The reply to.
        /// </value>
        public Message ReplyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an instance, asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="messageFunc">The message function.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>
        /// A created instance task.
        /// </returns>
        public static async Task<MessagesViewModel> CreateAsync(int id, MessageCollection.MessageFunc messageFunc, int? groupId = null)
        {
            User currentUser = await YammerService.Instance.GetCurrentUserAsync();
            return new MessagesViewModel(id, currentUser, messageFunc, groupId);
        }

        /// <summary>
        /// Message posted update.
        /// </summary>
        /// <param name="responseFeed">The response feed.</param>
        public void MessagePosted(MessageFeed responseFeed)
        {
            this.Messages.Insert(0, responseFeed.Messages.First());
        }

        /// <summary>
        /// Called when this instance needs to refresh, asynchronously.
        /// </summary>
        /// <returns>
        /// A completion task.
        /// </returns>
        protected override Task OnRefreshAsync()
        {
            return ((MessageCollection)this.Messages).LoadNewItemsAsync();
        }
    }
}
