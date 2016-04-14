//---------------------------------------------------------------------
// <copyright file="YModuleAttachment.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// YModule attachment class
    /// </summary>
    public class YModuleAttachment : IAttachment
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("fake_ogo_ymodule")]
        public bool FakeOgoYmodule { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("record_id")]
        public long RecordId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("network_id")]
        public int NetworkId { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("thumbnail_url")]
        public object ThumbnailUrl { get; set; }

        [JsonProperty("object_type")]
        public string ObjectType { get; set; }

        [JsonProperty("object_name")]
        public object ObjectName { get; set; }

        [JsonProperty("host_url")]
        public object HostUrl { get; set; }

        [JsonProperty("inline_url")]
        public string InlineUrl { get; set; }

        [JsonProperty("inline_html")]
        public string InlineHtml { get; set; }
    }

}
