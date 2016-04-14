//---------------------------------------------------------------------
// <copyright file="LikedBy.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Liked by class
    /// </summary>
    public class LikedBy
    {
        public int Count { get { return this.Names.Count; } }

        [JsonProperty("names")]
        public IList<User> Names { get; set; }

        public class User
        {
            [JsonProperty("full_name")]
            public string FullName { get; set; }

            [JsonProperty("permalink")]
            public string Permalink { get; set; }

            [JsonProperty("user_id")]
            public int UserId { get; set; }

            [JsonProperty("network_id")]
            public int NetworkId { get; set; }
        }
    }
}
