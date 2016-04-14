//---------------------------------------------------------------------
// <copyright file="User.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Diagnostics;
    using Newtonsoft.Json;

    /// <summary>
    /// User class
    /// </summary>
    [DebuggerDisplay("User: {FullName}({Id})")]
    public class User : IReferenceObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("network_name")]
        public string NetworkName { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("mugshot_url")]
        public string MugshotUrl { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
