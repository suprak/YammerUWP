//---------------------------------------------------------------------
// <copyright file="SharedThread.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Shared thread class
    /// </summary>
    public class SharedThread : IReferenceObject
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

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
}
