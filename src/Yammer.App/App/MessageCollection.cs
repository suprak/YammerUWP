//---------------------------------------------------------------------
// <copyright file="MessageCollection.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml.Data;
    using Yammer.API.Models;

    /// <summary>
    /// Message collection class
    /// </summary>
    public class MessageCollection : ObservableCollection<Message>, ISupportIncrementalLoading, IDisposable
    {
        /// <summary>
        /// The maximum fetch size
        /// </summary>
        private const int MaxFetchSize = 20;

        /// <summary>
        /// The semaphore
        /// </summary>
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// The disposed flag
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageCollection" /> class.
        /// </summary>
        /// <param name="function">The function.</param>
        public MessageCollection(MessageFunc function)
        {
            this.Function = function.ThrowIfNull(nameof(function));
            this.HasMoreItems = true;
            this.MessageCache = new List<Message>(2 * MessageCollection.MaxFetchSize);
            this.semaphore = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// The message function.
        /// </summary>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The message feed.
        /// </returns>
        public delegate Task<MessageFeed> MessageFunc(int olderThanId, int newerThanId, int limit);

        /// <summary>
        /// Gets a value indicating whether this instance has more items.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has more items; otherwise, <c>false</c>.
        /// </value>
        public bool HasMoreItems
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <value>
        /// The function.
        /// </value>
        private MessageFunc Function
        {
            get;
        }

        /// <summary>
        /// Gets the message cache.
        /// </summary>
        /// <value>
        /// The message cache.
        /// </value>
        private List<Message> MessageCache
        {
            get;
        }

        /// <summary>
        /// Gets or sets the last fetch index.
        /// </summary>
        /// <value>
        /// The last fetch index.
        /// </value>
        private int LastFetchIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last fetch.
        /// </summary>
        /// <value>
        /// The last fetch.
        /// </value>
        private DateTimeOffset LastFetch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the oldest identifier.
        /// </summary>
        /// <value>
        /// The oldest thread identifier.
        /// </value>
        private int OldestId
        {
            get;
            set;
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        protected override void InsertItem(int index, Message item)
        {
            base.InsertItem(index, item);

            // should only matter if posting to any empty group?
            if(this.OldestId == -1)
            {
                this.OldestId = item.Id;
            }
        }

        /// <summary>
        /// Loads the more items asynchronous.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The load more items result operation.
        /// </returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return this.InternalLoadMoreItemsAsync((int)count).AsAsyncOperation();
        }

        /// <summary>
        /// Loads the more items, asynchronously.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        public async Task LoadMoreItemsAsync(bool force)
        {
            await this.semaphore.WaitAsync();
            try
            {
                if (force || !this.MessageCache.Any() || DateTimeOffset.Now - this.LastFetch >= TimeSpan.FromSeconds(30))
                {
                    this.SetBusy(true);
                    MessageFeed feed = await this.Function(this.OldestId, -1, -1);

                    this.LastFetch = DateTimeOffset.Now;
                    if (feed.Messages.Any())
                    {
                        this.MessageCache.RemoveRange(0, this.LastFetchIndex);
                        this.LastFetchIndex = 0;
                        this.MessageCache.AddRange(feed.Messages);

                        this.OldestId = feed.Messages.Last().Id;
                    }

                    this.HasMoreItems = feed.Messages.Any();
                    this.SetBusy(false);
                }
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Loads the new items, asynchronously.
        /// </summary>
        /// <returns>
        /// The completion task.
        /// </returns>
        public async Task LoadNewItemsAsync()
        {
            await this.semaphore.WaitAsync();
            try
            {
                this.OldestId = -1;
                this.MessageCache.Clear();
                this.LastFetchIndex = 0;
                this.LastFetch = DateTimeOffset.MinValue;
                this.Clear();
                this.HasMoreItems = true;
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.semaphore?.Dispose();
                this.disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal load more items operation, asynchronous.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The load more items result operation.
        /// </returns>
        private async Task<LoadMoreItemsResult> InternalLoadMoreItemsAsync(int count)
        {
            if (this.MessageCache.Count - this.LastFetchIndex < count)
            {
                await this.LoadMoreItemsAsync(false);
            }

            int read = 0;
            for (int i = this.LastFetchIndex; i < this.MessageCache.Count && read < count; i++)
            {
                this.Add(this.MessageCache[i]);
                read++;
            }

            this.LastFetchIndex += read;
            return new LoadMoreItemsResult() { Count = (uint)read };
        }

        /// <summary>
        /// Sets the busy.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetBusy(bool value)
        {
            this.IsBusy = value;
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsBusy)));
        }
    }
}
