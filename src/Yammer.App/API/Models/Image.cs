//---------------------------------------------------------------------
// <copyright file="Image.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Image class
    /// </summary>
    public class Image
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }

}
