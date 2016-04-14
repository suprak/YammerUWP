//---------------------------------------------------------------------
// <copyright file="Topic.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API.Models
{
    using Newtonsoft.Json;

    public class Topic : IReferenceObject
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
