//---------------------------------------------------------------------
// <copyright file="YammerService.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Windows.Storage;
    using Windows.Storage.FileProperties;
    using Yammer.API.Models;

    /// <summary>
    /// Yammer service class
    /// </summary>
    public partial class YammerService : IDisposable
    {
        /// <summary>
        /// The threaded values
        /// </summary>
        public static readonly Dictionary<Threaded, string> ThreadedValues = Enum.GetValues(typeof(Threaded)).Cast<Threaded>().ToDictionary(_ => _, _ => _.ToString().ToLower());

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Prevents a default instance of the <see cref="YammerService"/> class from being created.
        /// </summary>
        private YammerService()
        {
            this.MessageHandler = new CustomMessageHandler();
            this.Client = new HttpClient(this.MessageHandler, true);
            this.Client.BaseAddress = new Uri("https://www.yammer.com/api/v1/");

            this.Client.DefaultRequestHeaders.Accept.Clear();
            this.Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        }

        /// <summary>
        /// Threaded enum
        /// </summary>
        public enum Threaded
        {
            True,
            Extended,
            False
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static YammerService Instance { get; } = new YammerService();

        /// <summary>
        /// Gets the dynamic object converter.
        /// </summary>
        /// <value>
        /// The dynamic object converter.
        /// </value>
        private static JsonVirtualConverter<IReferenceObject> DynamicObjectConverter { get; } = new JsonVirtualConverter<IReferenceObject>("type", (type) =>
                {
                    IReferenceObject result = null;
                    switch (type)
                    {
                        case "user":
                            result = new User();
                            break;
                        case "conversation":
                            result = new Conversation();
                            break;
                        case "shared_thread":
                            result = new SharedThread();
                            break;
                        case "shared_message":
                            result = new SharedMessage();
                            break;
                        case "message":
                            result = new Message();
                            break;
                        case "thread":
                            result = new Thread();
                            break;
                        case "group":
                            result = new Group();
                            break;
                        case "tag":
                            result = new Tag();
                            break;
                        case "topic":
                            result = new Topic();
                            break;
                        case "guide":
                            result = new Guide();
                            break;
                        default:
                            break;
                    }

                    return result;
                });

        /// <summary>
        /// Gets the attachment object resolver.
        /// </summary>
        /// <value>
        /// The attachment object resolver.
        /// </value>
        private static JsonVirtualConverter<IAttachment> AttachmentObjectResolver { get; } = new JsonVirtualConverter<IAttachment>("type", (type) =>
                {
                    IAttachment result = null;
                    switch (type)
                    {
                        case "image":
                            result = new ImageAttachment();
                            break;
                        case "message":
                            result = new MessageAttachment();
                            break;
                        // TODO: Support below in the future
                        //case "ymodule":
                        //    result = new YModuleAttachment();
                        //    break;
                        //case "praise":
                        //    result = new PraiseAttachment();
                        //    break;
                        default:
                            break;
                    }

                    return result;
                });

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        private HttpClient Client
        {
            get;
        }

        /// <summary>
        /// Gets the message handler.
        /// </summary>
        /// <value>
        /// The message handler.
        /// </value>
        private CustomMessageHandler MessageHandler
        {
            get;
        }

        /// <summary>
        /// Sets the active network.
        /// </summary>
        /// <param name="network">The network.</param>
        public void SetActiveNetwork(Network network)
        {
            this.MessageHandler.ActiveNetwork = network.ThrowIfNull(nameof(network));
        }

        /// <summary>
        /// Gets the current user, asynchronously.
        /// </summary>
        /// <returns>
        /// The current user task.
        /// </returns>
        public async Task<User> GetCurrentUserAsync()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "users/current.json"))
            {
                request.Headers.Add(CustomMessageHandler.CacheNameHeader, "users_current.json");
                request.Headers.Add(CustomMessageHandler.CacheMaxAgeHeader, TimeSpan.FromDays(1).ToString());
                using (HttpResponseMessage response = await this.Client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    return await YammerService.GetResponseContentAsync<User>(response);
                }
            }
        }

        /// <summary>
        /// Gets the network, asynchronously.
        /// </summary>
        /// <returns>
        /// The network task.
        /// </returns>
        public async Task<Network[]> GetNetworkAsync()
        {
            using (HttpResponseMessage tokensResponse = await this.Client.GetAsync("oauth/tokens.json", HttpCompletionOption.ResponseContentRead))
            using (HttpResponseMessage networksResponse = await this.Client.GetAsync("networks/current?exclude_own_messages_from_unseen=true&include_group_counts=true&inbox_supported_client=true&exclude_private_unread_thread_count=true", HttpCompletionOption.ResponseContentRead))
            {
                NetworkToken[] tokens = await YammerService.GetResponseContentAsync<NetworkToken[]>(tokensResponse);
                Network[] networks = await YammerService.GetResponseContentAsync<Network[]>(networksResponse);

                Dictionary<int, NetworkToken> tokenLookup = tokens.ToDictionary(_ => _.NetworkId);
                foreach (Network network in networks)
                {
                    NetworkToken token;
                    if (tokenLookup.TryGetValue(network.Id, out token))
                    {
                        network.Token = token;

                        if (network.GroupCounts != null)
                        {
                            // some custom logic to derive the most frequently used group order + unseen counts
                            network.GroupCounts.UnseenGroupThreadCountsLookup = new Dictionary<int, Tuple<int, int>>();
                            int index = 0;
                            foreach (JProperty property in network.GroupCounts.UnseenGroupThreadCounts.Properties())
                            {
                                network.GroupCounts.UnseenGroupThreadCountsLookup[int.Parse(property.Name)] = Tuple.Create(index++, (int)(long)((JValue)property.Value).Value);
                            }
                        }
                    }

                    if (network.IsPrimary && this.MessageHandler.ActiveNetwork == null)
                    {
                        this.SetActiveNetwork(network);
                    }
                }

                return networks;
            }
        }

        /// <summary>
        /// Gets the user groups, asynchronously.
        /// </summary>
        /// <returns>
        /// The groups task.
        /// </returns>
        public async Task<Group[]> GetUserGroupsAsync()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "groups.json?mine=1"))
            {
                request.Headers.Add(CustomMessageHandler.CacheNameHeader, "my_groups.json");
                request.Headers.Add(CustomMessageHandler.CacheMaxAgeHeader, TimeSpan.FromDays(1).ToString());
                using (HttpResponseMessage response = await this.Client.SendAsync(request, HttpCompletionOption.ResponseContentRead))
                {
                    return await YammerService.GetResponseContentAsync<Group[]>(response);
                }
            }
        }

        /// <summary>
        /// Gets the message, asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The message task.
        /// </returns>
        public async Task<Message> GetMessageAsync(int id)
        {
            using (HttpResponseMessage response = await this.Client.GetAsync(string.Concat("messages/", id, ".json"), HttpCompletionOption.ResponseContentRead))
            {
                return await YammerService.GetResponseContentAsync<Message>(response);
            }
        }

        /// <summary>
        /// Gets the thread, asynchronously.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The thread task.
        /// </returns>
        public async Task<Thread> GetThreadAsync(int id)
        {
            using (HttpResponseMessage response = await this.Client.GetAsync(string.Concat("threads/", id, ".json"), HttpCompletionOption.ResponseContentRead))
            {
                return await YammerService.GetResponseContentAsync<Thread>(response);
            }
        }

        /// <summary>
        /// Gets my message feed, asynchronously.
        /// </summary>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The my message feed task.
        /// </returns>
        public Task<MessageFeed> GetMyFeedAsync(int olderThanId = -1, int newerThanId = -1, int limit = -1)
        {
            return this.GetMessagesAsync("messages/my_feed.json", Threaded.Extended, olderThanId, newerThanId, limit);
        }

        /// <summary>
        /// Gets the received messages, asynchronously.
        /// </summary>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The received messages.
        /// </returns>
        public Task<MessageFeed> GetReceivedMessagesAsync(int olderThanId = -1, int newerThanId = -1, int limit = -1)
        {
            return this.GetMessagesAsync("messages/inbox.json", Threaded.Extended, olderThanId, newerThanId, limit);
        }

        /// <summary>
        /// Gets the group messages, asynchronously.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The group messages.
        /// </returns>
        public Task<MessageFeed> GetGroupMessagesAsync(int groupId, int olderThanId = -1, int newerThanId = -1, int limit = -1)
        {
            // SAMPLE
            // https://www.yammer.com/api/v1/messages/in_group/1614954.json?include_counts=true&limit=8&use_unviewed=true&threaded=extended&exclude_own_messages_from_unseen=true&_=1447382712193
            return this.GetMessagesAsync(string.Concat("messages/in_group/", groupId, ".json"), Threaded.Extended, olderThanId, newerThanId, limit);
        }

        /// <summary>
        /// Gets the conversation messages, asynchronously.
        /// </summary>
        /// <param name="threadId">The thread identifier.</param>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The conversation messages.
        /// </returns>
        public Task<MessageFeed> GetConversationMessagesAsync(int threadId, int olderThanId = -1, int newerThanId = -1, int limit = -1)
        {
            return this.GetMessagesAsync(string.Concat("messages/in_thread/", threadId, ".json"), Threaded.False, olderThanId, newerThanId, limit);
        }

        /// <summary>
        /// Likes the message, asynchronously.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        public async Task LikeMessageAsync(int messageId)
        {
            using (StringContent content = new StringContent(string.Empty))
            using (HttpResponseMessage response = await this.Client.PostAsync(YammerService.BuildRequest("messages/liked_by/current.json", "message_id", messageId.ToString()), content))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        /// Unlikes message, asynchronously.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        public async Task UnLikeMessageAsync(int messageId)
        {
            using (HttpResponseMessage response = await this.Client.DeleteAsync(YammerService.BuildRequest("messages/liked_by/current.json", "message_id", messageId.ToString())))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        /// Gets the private resource stream, asynchronously.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The resource stream task.
        /// </returns>
        public async Task<Stream> GetPrivateResourceStreamAsync(string uri)
        {
            uri.ThrowIfEmpty(nameof(uri));
            using (HttpResponseMessage response = await this.Client.GetAsync(uri, HttpCompletionOption.ResponseContentRead))
            {
                response.EnsureSuccessStatusCode();
                MemoryStream stream = new MemoryStream();
                await response.Content.CopyToAsync(stream);
                stream.Position = 0;
                return stream;
            }
        }

        /// <summary>
        /// Gets the image attachment file, asynchronously.
        /// </summary>
        /// <param name="targetUrl">The target URL.</param>
        /// <param name="attachment">The attachment.</param>
        /// <returns>
        /// The image attachment file task.
        /// </returns>
        public async Task<IStorageFile> GetImageAttachmentFileAsync(string targetUrl, ImageAttachment attachment)
        {
            targetUrl.ThrowIfEmpty(nameof(targetUrl));
            attachment.ThrowIfNull(nameof(attachment));

            StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
            StorageFolder cacheFolder = await tempFolder.CreateFolderAsync("Cache", CreationCollisionOption.OpenIfExists);
            StorageFolder attachmentsFolder = await cacheFolder.CreateFolderAsync("Attachments", CreationCollisionOption.OpenIfExists);
            StorageFile attachmentFile = null;

            // use only the id since it's a safer name as otherwise there may be unsafe characters to prune?
            string id = string.Concat(attachment.Id.ToString(), Path.GetExtension(attachment.Name));

            try
            {
                attachmentFile = await attachmentsFolder.CreateFileAsync(id, CreationCollisionOption.FailIfExists);
                using (HttpResponseMessage response = await this.Client.GetAsync(targetUrl, HttpCompletionOption.ResponseContentRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (Stream stream = await attachmentFile.OpenStreamForWriteAsync())
                    {
                        await response.Content.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception e) when (e.HResult == -2147024713)
            {
                attachmentFile = await attachmentsFolder.GetFileAsync(id);
            }

            return attachmentFile;
        }

        /// <summary>
        /// Posts the message, asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The message feed task.
        /// </returns>
        public async Task<MessageFeed> PostMessageAsync(PostRequest request)
        {
            request.ThrowIfNull(nameof(request));

            User currentUser = await this.GetCurrentUserAsync();
            string json = JsonConvert.SerializeObject(request);
            using (HttpContent content = new StringContent(json))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                using (HttpResponseMessage response = await this.Client.PostAsync(YammerService.BuildRequest("messages.json"), content))
                {
                    MessageFeed feed = await YammerService.GetResponseContentAsync<MessageFeed>(response);
                    return YammerService.ProcessMessages(currentUser, feed);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.Client != null)
                {
                    this.Client.Dispose();
                }

                this.disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the activity feed, asynchronously.
        /// </summary>
        /// <param name="olderThanId">The older than identifier.</param>
        /// <param name="newerThanId">The newer than identifier.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>
        /// The activity feed task.
        /// </returns>
        private async Task<MessageFeed> GetMessagesAsync(string uri, Threaded? threaded, int olderThanId, int newerThanId, int limit)
        {
            List<string> parameters = new List<string>(3);

            if (threaded.HasValue)
            {
                parameters.Add("threaded");
                parameters.Add(YammerService.ThreadedValues[threaded.Value]);
            }

            if (limit > 0)
            {
                parameters.Add("limit");
                parameters.Add(limit.ToString());
            }

            if (olderThanId > 0)
            {
                parameters.Add("older_than");
                parameters.Add(olderThanId.ToString());
            }

            if (newerThanId > 0)
            {
                parameters.Add("newer_than");
                parameters.Add(newerThanId.ToString());
            }

            // forces all loaded messages to be marked read
            parameters.Add("update_last_seen_message_id");
            parameters.Add("true");

            User currentUser = await this.GetCurrentUserAsync();
            using (HttpResponseMessage response = await this.Client.GetAsync(YammerService.BuildRequest(uri, parameters.ToArray()), HttpCompletionOption.ResponseContentRead))
            {
                MessageFeed feed = await YammerService.GetResponseContentAsync<MessageFeed>(response);
                return YammerService.ProcessMessages(currentUser, feed);
            }
        }

        /// <summary>
        /// Processes the messages.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="feed">The feed.</param>
        /// <returns>
        /// The processed messages.
        /// </returns>
        private static MessageFeed ProcessMessages(User currentUser, MessageFeed feed)
        {
            currentUser.ThrowIfNull(nameof(currentUser));
            feed.ThrowIfNull(nameof(feed));

            Dictionary<int, User> users = feed.References.OfType<User>().ToDictionary(_ => _.Id);
            Dictionary<int, Thread> threads = feed.References.OfType<Thread>().ToDictionary(_ => _.Id);
            Dictionary<int, Group> groups = feed.References.OfType<Group>().ToDictionary(_ => _.Id);
            Dictionary<int, Message> messages = feed.References.OfType<Message>().ToDictionary(_ => _.Id);
            HashSet<int> likedMessageIds = new HashSet<int>();

            if (feed.Meta != null && feed.Meta.LikedMessageIds != null)
            {
                foreach (int id in feed.Meta.LikedMessageIds)
                {
                    likedMessageIds.Add(id);
                }
            }

            foreach (Message message in feed.Messages)
            {
                User user;
                if (users.TryGetValue(message.SenderId, out user))
                {
                    message.Sender = user;
                }

                Thread thread;
                if (threads.TryGetValue(message.ThreadId, out thread))
                {
                    message.Thread = thread;
                }

                Group group;
                if (message.GroupId.HasValue && groups.TryGetValue(message.GroupId.Value, out group))
                {
                    message.Group = group;
                }

                Message[] children;
                feed.ThreadedExtended.TryGetValue(message.Id, out children);
                message.TimeSinceLatestActivity = DateTimeOffset.Now - (children == null || !children.Any() ? message.CreatedAt : children.Max(_ => _.CreatedAt));

                Message repliedTo;
                User repliedToUser;
                if (message.RepliedToId.HasValue &&
                    messages.TryGetValue(message.RepliedToId.Value, out repliedTo) &&
                    users.TryGetValue(repliedTo.SenderId, out repliedToUser))
                {
                    message.RepliedToUser = repliedToUser;
                }

                message.IsLiked = message.LikedBy.Names.Where(_ => _.UserId == currentUser.Id).Any();
                if (!message.IsLiked && likedMessageIds.Contains(message.Id))
                {
                    message.IsLiked = true;
                    message.LikedBy.Names.Add(new LikedBy.User() { FullName = currentUser.FullName, NetworkId = currentUser.NetworkId, UserId = currentUser.Id });
                }

                message.SetSupportedAttachments(message.Attachments.Where(_ => _ != null).ToArray());
            }

            return feed;
        }

        /// <summary>
        /// Builds the request.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <param name="apiVersion">The API version.</param>
        /// <returns>
        /// A built request URI.
        /// </returns>
        private static string BuildRequest(string baseUri, params string[] queryParameters)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(baseUri);
            if (queryParameters.Length > 0 && queryParameters.Length % 2 == 0)
            {
                builder.Append('?');
                for (int i = 0; i < queryParameters.Length; i += 2)
                {
                    builder.Append(queryParameters[i]).Append('=').Append(queryParameters[i + 1]).Append('&');
                }

                builder.Length--;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the content of the response, asynchornously.
        /// </summary>
        /// <typeparam name="TContent">The type of the content.</typeparam>
        /// <param name="response">The response.</param>
        /// <param name="callerName">Name of the caller.</param>
        /// <returns>
        /// The content object.
        /// </returns>
        private static async Task<TContent> GetResponseContentAsync<TContent>(HttpResponseMessage response, [CallerMemberName] string callerName = "")
        {
            string content = await response.Content.ReadAsStringAsync();
#if DEBUG
            if (!response.IsSuccessStatusCode && System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
#else
            response.EnsureSuccessStatusCode();
#endif

            return JsonConvert.DeserializeObject<TContent>(content, YammerService.DynamicObjectConverter, YammerService.AttachmentObjectResolver);
        }

        /// <summary>
        /// Provides virtual type instatiation based on the value of a type attribute.
        /// </summary>
        /// <typeparam name="T">Base class for any object instantiated based on the type attribute.</typeparam>
        private class JsonVirtualConverter<T> : JsonConverter
        {
            /// <summary>
            /// Initialization Contructor.
            /// </summary>
            /// <param name="typeProperty">The type property.</param>
            /// <param name="typeResolver">Type-factory function to instantiate a new object based on the value of the type attribute.</param>
            public JsonVirtualConverter(string typeProperty, Func<string, T> typeResolver)
            {
                this.TypeProperty = typeProperty.ThrowIfEmpty(nameof(typeProperty));
                this.TypeResolver = typeResolver.ThrowIfNull(nameof(typeResolver));
            }

            /// <summary>
            /// Gets a value indicating whether this instance can write.
            /// </summary>
            /// <value>
            /// returns <c>true</c> if this instance can write; otherwise, <c>false</c>.
            /// </value>
            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Gets the type property.
            /// </summary>
            /// <value>
            /// The type property.
            /// </value>
            private string TypeProperty
            {
                get;
            }

            /// <summary>
            /// Gets the type resolver.
            /// </summary>
            /// <value>
            /// The type resolver.
            /// </value>
            private Func<string, T> TypeResolver
            {
                get;
            }

            /// <summary>
            /// Determines whether this instance can convert the specified object type.
            /// </summary>
            /// <param name="objectType">Type of the object.</param>
            /// <returns>
            /// true, if this instance can convert the specified object type; otherwise, false.
            /// </returns>
            public override bool CanConvert(Type objectType)
            {
                return typeof(T).IsAssignableFrom(objectType);
            }

            /// <summary>
            /// Reads the JSON representation of the object.
            /// </summary>
            /// <param name="reader">The JsonReader object to read from.</param>
            /// <param name="objectType">Type of the object.</param>
            /// <param name="existingValue">The existing value of object being read.</param>
            /// <param name="serializer">The calling serializer.</param>
            /// <returns>The object value.</returns>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject obj = JObject.Load(reader);
                string value = (string)obj.Property(this.TypeProperty);
                T target = this.TypeResolver(value);
                if (value == null && target == null)
                {
                    target = (T)Activator.CreateInstance(objectType);
                }
                else if (target == null)
                {
                    while (reader.TokenType != JsonToken.EndObject && reader.Read()) ;
                }

                if (target != null)
                {
                    serializer.Populate(obj.CreateReader(), target);
                }

                return target;
            }

            /// <summary>
            /// Writes the JSON representation of the object.
            /// </summary>
            /// <param name="writer">The JsonWriter object to write to.</param>
            /// <param name="value">The value.</param>
            /// <param name="serializer">The calling serializer.</param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotSupportedException("JsonVirtualConverter should only be used while deserializing.");
            }
        }

        /// <summary>
        /// Custom message handler class
        /// </summary>
        private class CustomMessageHandler : HttpClientHandler
        {
            /// <summary>
            /// The cache name header.
            /// </summary>
            public const string CacheNameHeader = "x-app-cache-name";

            /// <summary>
            /// The cache maximum age header.
            /// </summary>
            public const string CacheMaxAgeHeader = "x-app-cache-maxage";

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomMessageHandler" /> class.
            /// </summary>
            public CustomMessageHandler()
            {
                this.UseCookies = false;
                if (this.SupportsAutomaticDecompression)
                {
                    this.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

                this.Locks = new CacheLocks();
            }

            /// <summary>
            /// Gets or sets the active network.
            /// </summary>
            /// <value>
            /// The active network.
            /// </value>
            internal Network ActiveNetwork
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the token.
            /// </summary>
            /// <value>
            /// The token.
            /// </value>
            private OAuthToken Token
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the locks.
            /// </summary>
            /// <value>
            /// The locks.
            /// </value>
            private CacheLocks Locks
            {
                get;
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing">
            ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.Locks?.Dispose();
                }
            }

            /// <summary>
            /// Sends the request, asynchronously.
            /// </summary>
            /// <param name="request">The request.</param>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// The response message.
            /// </returns>
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = null;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                SemaphoreSlim semaphore = null;
                StorageFile cacheFile = null;
                bool cacheHit = false;
                bool cacheResponse = false;
                if (request.Headers.Contains(CustomMessageHandler.CacheNameHeader))
                {
                    string cacheFileName = request.Headers.GetValues(CustomMessageHandler.CacheNameHeader).First();
                    semaphore = await this.Locks.GetLockAsync(cacheFileName);

                    cacheResponse = true;
                    TimeSpan maxAge = TimeSpan.MaxValue;
                    IEnumerable<string> maxAgeValues;
                    if (!request.Headers.TryGetValues(CustomMessageHandler.CacheMaxAgeHeader, out maxAgeValues) ||
                        !TimeSpan.TryParse(maxAgeValues.First(), out maxAge))
                    {
                        maxAge = TimeSpan.MaxValue;
                    }

                    await semaphore.WaitAsync();

                    bool cacheExists = true;
                    bool fileFound = false;

                    TimeSpan fileAge = TimeSpan.Zero;

                    try
                    {
                        StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
                        StorageFolder cacheFolder = await tempFolder.CreateFolderAsync("Cache", CreationCollisionOption.OpenIfExists);
                        if(this.ActiveNetwork != null)
                        {
                            cacheFolder = await cacheFolder.CreateFolderAsync($"{this.ActiveNetwork.Id}", CreationCollisionOption.OpenIfExists);
                        }

                        try
                        {
                            cacheFile = await cacheFolder.GetFileAsync(cacheFileName);
                            BasicProperties properties = await cacheFile.GetBasicPropertiesAsync();

                            fileFound = properties.Size > 0;
                            fileAge = DateTimeOffset.Now - properties.DateModified;
                        }
                        catch (FileNotFoundException)
                        {
                        }

                        if (!fileFound)
                        {
                            cacheFile = await cacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.OpenIfExists);
                            cacheExists = false;
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }

                    if (cacheExists && fileAge < maxAge)
                    {
                        response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Content = new StreamContent(await cacheFile.OpenStreamForReadAsync());
                        cacheHit = true;
                    }
                }

                if (!cacheHit)
                {
                    OAuthToken token;
                    if (this.Token == null && OAuthToken.TryGetValue(out token))
                    {
                        this.Token = token;
                    }

                    if (this.Token == null)
                    {
                        throw new InvalidOperationException("No OAuth token present.");
                    }

                    string tokenValue = this.Token.Value;
                    if (this.ActiveNetwork != null)
                    {
                        tokenValue = this.ActiveNetwork.Token.Value;
                    }

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
                    response = await base.SendAsync(request, cancellationToken);

                    if (cacheResponse && cacheFile != null)
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            await FileIO.WriteBytesAsync(cacheFile, await response.Content.ReadAsByteArrayAsync());
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                }

                stopwatch.Stop();

                Logger.Instance.LogEvent(
                        "API Call",
                        new Dictionary<string, string> {
                            { "api.path", request.RequestUri.AbsolutePath },
                            { "response.code", $"{response.StatusCode:D}" },
                            { "cached", $"{cacheHit}" } },
                        new Dictionary<string, double> {
                            { "request.latency", stopwatch.ElapsedMilliseconds } });

                return response;
            }
        }

        /// <summary>
        /// Cache locks class
        /// </summary>
        public class CacheLocks : IDisposable
        {
            /// <summary>
            /// The locks lock
            /// </summary>
            private readonly SemaphoreSlim locksLock;

            /// <summary>
            /// The locks
            /// </summary>
            private readonly Dictionary<string, SemaphoreSlim> locks;

            /// <summary>
            /// The disposed flag
            /// </summary>
            private bool disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheLocks"/> class.
            /// </summary>
            public CacheLocks()
            {
                this.locksLock = new SemaphoreSlim(1, 1);
                this.locks = new Dictionary<string, SemaphoreSlim>(StringComparer.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Gets the lock, asynchronously.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns>
            /// The lock task.
            /// </returns>
            public async Task<SemaphoreSlim> GetLockAsync(string name)
            {
                SemaphoreSlim result;
                if (!this.locks.TryGetValue(name, out result))
                {
                    await this.locksLock.WaitAsync();
                    try
                    {
                        if (!this.locks.TryGetValue(name, out result))
                        {
                            this.locks[name] = result = new SemaphoreSlim(1, 1);
                        }
                    }
                    finally
                    {
                        this.locksLock.Release();
                    }
                }

                return result;
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            public void Dispose()
            {
                if (!this.disposed)
                {
                    this.locksLock?.Dispose();
                    if (this.locks != null)
                    {
                        foreach (SemaphoreSlim semaphore in this.locks.Values)
                        {
                            semaphore?.Dispose();
                        }

                        this.locks.Clear();
                    }

                    this.disposed = true;
                }

                GC.SuppressFinalize(this);
            }
        }
    }
}
