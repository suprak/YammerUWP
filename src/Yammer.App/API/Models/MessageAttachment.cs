//---------------------------------------------------------------------
// <copyright file="MessageAttachment.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Message attatchment class
    /// </summary>
    public class MessageAttachment : IAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sender_id")]
        public int SenderId { get; set; }

        [JsonProperty("replied_to_id")]
        public object RepliedToId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        [JsonProperty("sender_type")]
        public string SenderType { get; set; }

        [JsonProperty("thread_id")]
        public int ThreadId { get; set; }

        [JsonProperty("message_type")]
        public string MessageType { get; set; }

        [JsonProperty("system_message")]
        public bool SystemMessage { get; set; }

        [JsonProperty("content_excerpt")]
        public string ContentExcerpt { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("direct_message")]
        public bool DirectMessage { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }
}
}
