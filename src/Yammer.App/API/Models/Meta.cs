//---------------------------------------------------------------------
// <copyright file="Meta.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Meta class
    /// </summary>
    public class Meta
    {
        [JsonProperty("last_seen_message_id")]
        public int? LastSeenMessageId { get; set; }

        [JsonProperty("liked_message_ids")]
        public IList<int> LikedMessageIds { get; set; }

        [JsonProperty("unseen_thread_count")]
        public object UnseenThreadCount { get; set; }

        [JsonProperty("unseen_message_count_by_thread")]
        public IDictionary<int, int> UnseenMessageCountByThread { get; set; }
    }
}
