//---------------------------------------------------------------------
// <copyright file="Message.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Message class
    /// </summary>
    [DebuggerDisplay("Message: {Id}")]
    public class Message : IReferenceObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sender_id")]
        public int SenderId { get; set; }

        public User Sender { get; set; }

        [JsonProperty("replied_to_id")]
        public int? RepliedToId { get; set; }

        public User RepliedToUser { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("message_type")]
        public string MessageType { get; set; }

        [JsonProperty("sender_type")]
        public string SenderType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("thread_id")]
        public int ThreadId { get; set; }

        public Thread Thread { get; set; }

        [JsonProperty("client_type")]
        public string ClientType { get; set; }

        [JsonProperty("client_url")]
        public string ClientUrl { get; set; }

        [JsonProperty("system_message")]
        public bool SystemMessage { get; set; }

        [JsonProperty("direct_message")]
        public bool DirectMessage { get; set; }

        [JsonProperty("chat_client_sequence")]
        public object ChatClientSequence { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("attachments")]
        public IAttachment[] Attachments { get; set; }

        [JsonProperty("liked_by")]
        public LikedBy LikedBy { get; set; }

        [JsonProperty("content_excerpt")]
        public string ContentExcerpt { get; set; }

        [JsonProperty("group_created_id")]
        public object GroupCreatedId { get; set; }

        [JsonProperty("group_id")]
        public int? GroupId { get; set; }

        public Group Group { get; set; }

        /// <summary>
        /// Gets the time since latest activity.
        /// </summary>
        /// <value>
        /// The time since latest activity.
        /// </value>
        public TimeSpan TimeSinceLatestActivity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is liked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is liked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLiked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the supported attachments.
        /// </summary>
        /// <value>
        /// The supported attachments.
        /// </value>
        public IAttachment[] SupportedAttachments { get; private set; }

        /// <summary>
        /// Gets the image attachments.
        /// </summary>
        /// <value>
        /// The image attachments.
        /// </value>
        public IEnumerable<ImageAttachment> ImageAttachments { get; private set; }

        /// <summary>
        /// Gets the message attachment.
        /// </summary>
        /// <value>
        /// The message attachment.
        /// </value>
        public MessageAttachment MessageAttachment { get; private set; }

        /// <summary>
        /// Sets the supported attachments.
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        public void SetSupportedAttachments(IAttachment[] attachments)
        {
            this.SupportedAttachments = attachments.ThrowIfNull(nameof(attachments));
            this.ImageAttachments = this.SupportedAttachments.OfType<ImageAttachment>().ToArray();
            this.MessageAttachment = this.SupportedAttachments.OfType<MessageAttachment>().FirstOrDefault();
        }

        /// <summary>
        /// Triggers the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void TriggerPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
