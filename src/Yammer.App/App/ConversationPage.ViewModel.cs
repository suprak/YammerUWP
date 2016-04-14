//---------------------------------------------------------------------
// <copyright file="ConversationPage.ViewModel.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using API;
    using API.Models;

    /// <summary>
    /// Conversation page class
    /// </summary>
    public partial class ConversationPage
    {
        /// <summary>
        /// View model class
        /// </summary>
        public class ViewModel : ViewModelBase, IPostModel, IDisposable
        {
            /// <summary>
            /// The cache semaphore
            /// </summary>
            private readonly SemaphoreSlim cacheSemaphore;

            /// <summary>
            /// The disposed
            /// </summary>
            private bool disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewModel"/> class.
            /// </summary>
            /// <param name="arguments">The arguments.</param>
            public ViewModel(Arguments arguments)
                : base(TimeSpan.FromMinutes(5))
            {
                arguments.ThrowIfNull(nameof(arguments));

                if (arguments.Message != null)
                {
                    // this also happens below in PopulateCacheAsync
                    arguments.Message.TimeSinceLatestActivity = DateTimeOffset.Now - arguments.Message.CreatedAt;

                    this.ThreadMessage = arguments.Message;
                    this.MessageId = arguments.Message.Id;
                    this.ThreadId = arguments.Message.ThreadId;
                    this.ReplyTo = this.ThreadMessage;
                }
                else
                {
                    this.MessageId = arguments.MessageAttachment.Id;
                    this.ThreadId = arguments.MessageAttachment.ThreadId;
                }

                this.Messages = new ObservableCollection<Message>();
                this.MessageCache = new Queue<Message>();
                this.OldestMessageId = -1;
                this.HasMoreMessages = false;
                this.ReplyMessage = string.Empty;

                this.cacheSemaphore = new SemaphoreSlim(1, 1);
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
            /// Gets the thread message.
            /// </summary>
            /// <value>
            /// The thread message.
            /// </value>
            public Message ThreadMessage
            {
                get { return this.GetProperty<Message>(); }
                private set { this.SetProperty(value); }
            }

            /// <summary>
            /// Gets the messages.
            /// </summary>
            /// <value>
            /// The messages.
            /// </value>
            public IList<Message> Messages
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value indicating whether this instance has more messages.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance has more messages; otherwise, <c>false</c>.
            /// </value>
            public bool HasMoreMessages
            {
                get { return this.GetProperty<bool>(); }
                private set { this.SetProperty(value); }
            }

            /// <summary>
            /// Gets or sets the reply to.
            /// </summary>
            /// <value>
            /// The reply to.
            /// </value>
            public Message ReplyTo
            {
                get { return this.GetProperty<Message>(); }
                set { this.SetProperty(value); }
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
                get { return this.ThreadMessage.GroupId; }
            }

            /// <summary>
            /// Gets the message identifier.
            /// </summary>
            /// <value>
            /// The message identifier.
            /// </value>
            public int MessageId
            {
                get;
            }

            /// <summary>
            /// Gets the thread identifier.
            /// </summary>
            /// <value>
            /// The thread identifier.
            /// </value>
            public int ThreadId
            {
                get;
            }

            /// <summary>
            /// Gets the message cache.
            /// </summary>
            /// <value>
            /// The message cache.
            /// </value>
            private Queue<Message> MessageCache
            {
                get;
            }

            /// <summary>
            /// Gets or sets the oldest message identifier.
            /// </summary>
            /// <value>
            /// The oldest message identifier.
            /// </value>
            private int OldestMessageId
            {
                get;
                set;
            }

            /// <summary>
            /// Toggles the message like state, asynchronously.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <returns>
            /// The completion task.
            /// </returns>
            public async Task ToggleMessageLikeAsync(Message message)
            {
                await message.ToggleLikeAsync();
            }

            /// <summary>
            /// Message posted update.
            /// </summary>
            /// <param name="responseFeed">The response feed.</param>
            public void MessagePosted(MessageFeed responseFeed)
            {
                Message message = responseFeed.Messages.First();
                this.Messages.Add(message);
                this.ThreadMessage.Thread.Stats.Updates++;
                this.ThreadMessage.TriggerPropertyChanged(null);
            }

            /// <summary>
            /// Loads the more, asynchronously.
            /// </summary>
            /// <param name="initial">if set to <c>true</c> initial load.</param>
            /// <returns>
            /// The completion task.
            /// </returns>
            public async Task LoadMoreAsync(bool initial = false)
            {
                bool performAsync = false;
                int limit = initial ? 2 : 10;

                // if we don't have enough messages cached to satisfy request
                // load more and wait for the load
                if (this.MessageCache.Count < limit)
                {
                    await this.PopulateCacheAsync();
                }
                else
                {
                    performAsync = true;
                }

                // make sure we don't try to yank more than exists (even post load)
                limit = Math.Min(limit, this.MessageCache.Count);

                this.IsBusy = true;
                await this.cacheSemaphore.WaitAsync();
                try
                {
                    while (limit > 0)
                    {
                        this.Messages.Insert(0, this.MessageCache.Dequeue());
                        limit--;
                    }
                }
                finally
                {
                    this.cacheSemaphore.Release();
                    this.IsBusy = false;
                }

                // if we didn't have to wait for a load
                // then now is a good time to go out and load some messages
                // that way when the user is ready, there will be zero delay for loading more
                if (performAsync)
                {
                    this.PopulateCache();
                }
                else
                {
                    this.HasMoreMessages = this.MessageCache.Any();
                }
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            public void Dispose()
            {
                if (!this.disposed)
                {
                    this.cacheSemaphore?.Dispose();

                    this.disposed = true;
                }

                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Called when this instance needs to refresh, asynchronously.
            /// </summary>
            /// <returns>
            /// A completion task.
            /// </returns>
            protected override async Task OnRefreshAsync()
            {
                this.CurrentUser = await YammerService.Instance.GetCurrentUserAsync();
                this.MessageCache.Clear();
                this.Messages.Clear();
                this.OldestMessageId = -1;
                this.HasMoreMessages = false;

                await this.LoadMoreAsync(true);
            }

            /// <summary>
            /// Populates the cache.
            /// </summary>
            private async void PopulateCache()
            {
                await this.PopulateCacheAsync();
            }

            /// <summary>
            /// Populates the cache asynchronous.
            /// </summary>
            /// <returns>
            /// The completion task.
            /// </returns>
            private async Task PopulateCacheAsync()
            {
                this.IsBusy = true;

                try
                {
                    MessageFeed feed = await YammerService.Instance.GetConversationMessagesAsync(this.ThreadId, this.OldestMessageId, -1, -1);

                    if (feed.Messages.Any())
                    {
                        await this.cacheSemaphore.WaitAsync();
                        try
                        {
                            foreach (Message message in feed.Messages)
                            {
                                if (message.Id != message.ThreadId)
                                {
                                    this.MessageCache.Enqueue(message);
                                }
                                else
                                {
                                    // Specifically avoiding doing the below as it will
                                    // cause a reloading of image attachments
                                    if (this.ThreadMessage == null)
                                    {
                                        // do this here as it only makes sense in conversation view to have the
                                        // main thread show when it was created rather than latest activity
                                        message.TimeSinceLatestActivity = DateTimeOffset.Now - message.CreatedAt;
                                        this.ThreadMessage = message;
                                    }
                                    else
                                    {
                                        this.ThreadMessage.LikedBy = message.LikedBy;
                                        this.ThreadMessage.Thread = message.Thread;
                                        this.ThreadMessage.TriggerPropertyChanged(null);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            this.cacheSemaphore.Release();
                        }

                        this.OldestMessageId = feed.Messages.Last().Id;
                    }

                    this.HasMoreMessages = this.MessageCache.Any();
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }
    }
}
