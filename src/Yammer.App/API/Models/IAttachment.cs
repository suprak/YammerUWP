//---------------------------------------------------------------------
// <copyright file="IAttachment.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Attachment interface
    /// </summary>
    public interface IAttachment
    {
        [JsonProperty("type")]
        string Type { get; set; }
    }
}
