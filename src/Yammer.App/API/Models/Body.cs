//---------------------------------------------------------------------
// <copyright file="Body.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Body class
    /// </summary>
    public class Body
    {
        [JsonProperty("urls")]
        public IList<string> Urls { get; set; }

        [JsonProperty("parsed")]
        public string Parsed { get; set; }

        [JsonProperty("plain")]
        public string Plain { get; set; }

        [JsonProperty("rich")]
        public string Rich { get; set; }

        /// <summary>
        /// Gets the smart body.
        /// </summary>
        /// <value>
        /// The smart body.
        /// </value>
        public string Smart { get { return string.IsNullOrEmpty(this.Plain) ? this.Parsed : this.Plain; } }
    }
}
