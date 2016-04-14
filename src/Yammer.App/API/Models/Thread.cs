//---------------------------------------------------------------------
// <copyright file="Thread.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Newtonsoft.Json;

    /// <summary>
    /// Thread class
    /// </summary>
    [DebuggerDisplay("Thread: {Id}")]
    public class Thread : IReferenceObject
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("thread_starter_id")]
        public int ThreadStarterId { get; set; }

        [JsonProperty("group_id")]
        public int? GroupId { get; set; }

        [JsonProperty("topics")]
        public IList<Topic> Topics { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("direct_message")]
        public bool DirectMessage { get; set; }

        [JsonProperty("has_attachments")]
        public bool HasAttachments { get; set; }

        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("invited_user_ids")]
        public IList<int> InvitedUserIds { get; set; }

        [JsonProperty("read_only")]
        public bool ReadOnly { get; set; }
    }

    /// <summary>
    /// Stats class
    /// </summary>
    public class Stats
    {
        [JsonProperty("updates")]
        public int Updates { get; set; }

        public int AdjustedUpdates { get { return this.Updates - 1; } }

        [JsonProperty("first_reply_id")]
        public int? FirstReplyId { get; set; }

        [JsonProperty("first_reply_at")]
        public DateTimeOffset? FirstReplyAt { get; set; }

        [JsonProperty("latest_reply_id")]
        public int? LatestReplyId { get; set; }

        [JsonProperty("latest_reply_at")]
        public DateTimeOffset? LatestReplyAt { get; set; }

        [JsonProperty("shares")]
        public int Shares { get; set; }
    }
}
