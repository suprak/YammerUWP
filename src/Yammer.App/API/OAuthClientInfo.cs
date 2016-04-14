//---------------------------------------------------------------------
// <copyright file="OAuthClientInfo.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API
{
    using System;

    /// <summary>
    /// OAuth client info class
    /// </summary>
    public class OAuthClientInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthClientInfo"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="callback">The callback.</param>
        public OAuthClientInfo(string clientId, Uri callback)
        {
            clientId.ThrowIfEmpty(nameof(clientId));
            callback.ThrowIfNull(nameof(callback));

            this.ClientId = clientId;
            this.Callback = callback;
            this.TargetUri = new Uri(string.Format("https://www.yammer.com/dialog/oauth?client_id={0}&redirect_uri={1}&response_type=token", this.ClientId, this.Callback));
        }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId
        {
            get;
        }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        public Uri Callback
        {
            get;
        }

        /// <summary>
        /// Gets the target URI.
        /// </summary>
        /// <value>
        /// The target URI.
        /// </value>
        public Uri TargetUri
        {
            get;
        }
    }
}
