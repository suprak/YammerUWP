//---------------------------------------------------------------------
// <copyright file="ThreadMessage.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Thread message class
    /// </summary>
    public class ThreadMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sender_id")]
        public int SenderId { get; set; }

        [JsonProperty("replied_to_id")]
        public int RepliedToId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

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

        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("thread_id")]
        public int ThreadId { get; set; }

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

        [JsonProperty("notified_user_ids")]
        public IList<object> NotifiedUserIds { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("attachments")]
        public IList<object> Attachments { get; set; }

        [JsonProperty("liked_by")]
        public LikedBy LikedBy { get; set; }

        [JsonProperty("content_excerpt")]
        public string ContentExcerpt { get; set; }
    }
}
