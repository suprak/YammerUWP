//---------------------------------------------------------------------
// <copyright file="Conversation.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ParticipatingName
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("mugshot_url")]
        public string MugshotUrl { get; set; }
    }

    public class Conversation : IReferenceObject
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("participating_users_count")]
        public int ParticipatingUsersCount { get; set; }

        [JsonProperty("participating_names")]
        public IList<ParticipatingName> ParticipatingNames { get; set; }
    }
}
