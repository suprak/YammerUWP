//---------------------------------------------------------------------
// <copyright file="MessageFeed.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// Message feed class
    /// </summary>
    public class MessageFeed
    {
        [JsonProperty("threaded_extended")]
        public IDictionary<int, Message[]> ThreadedExtended { get; set; }

        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("references")]
        public IReferenceObject[] References { get; set; }

        [JsonProperty("external_references")]
        public IReferenceObject[] ExternalReferences { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
