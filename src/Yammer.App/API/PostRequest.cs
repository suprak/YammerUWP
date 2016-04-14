//---------------------------------------------------------------------
// <copyright file="PostRequest.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API
{
    using Newtonsoft.Json;

    /// <summary>
    /// Post requests class
    /// </summary>
    public class PostRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostRequest"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public PostRequest(string body)
        {
            this.Body = body.ThrowIfEmpty(nameof(body));
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [JsonProperty("body")]
        public string Body
        {
            get;
        }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        [JsonProperty("group_id")]
        public int? GroupId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reply to message identifier.
        /// </summary>
        /// <value>
        /// The reply to message identifier.
        /// </value>
        [JsonProperty("replied_to_id")]
        public int? ReplyToId
        {
            get;
            set;
        }
    }
}
