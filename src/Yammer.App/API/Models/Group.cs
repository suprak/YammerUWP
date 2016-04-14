//---------------------------------------------------------------------
// <copyright file="Group.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Group class
    /// </summary>
    public class Group : IReferenceObject
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("mugshot_url")]
        public string MugshotUrl { get; set; }

        [JsonProperty("mugshot_url_template")]
        public string MugshotUrlTemplate { get; set; }

        [JsonProperty("mugshot_id")]
        public string MugshotId { get; set; }

        [JsonProperty("mugshot_color")]
        public string MugshotColor { get; set; }

        [JsonProperty("show_in_directory")]
        public string ShowInDirectory { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("members")]
        public int Members { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("external")]
        public bool External { get; set; }

        [JsonProperty("moderated")]
        public bool Moderated { get; set; }

        public int UnreadCount { get; set; }
    }
}
