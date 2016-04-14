//---------------------------------------------------------------------
// <copyright file="OAuthClient.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// OAuth client class
    /// </summary>
    public class OAuthClient
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="OAuthClient"/> class from being created.
        /// </summary>
        private OAuthClient()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static OAuthClient Instance { get; } = new OAuthClient();

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>
        /// The access token.
        /// </returns>
        public OAuthToken GetAccessToken(Uri payload)
        {
            OAuthToken token = null;
            string fragment = payload.ThrowIfNull(nameof(payload)).Fragment.TrimStart('#');
            Dictionary<string, string> fragments = fragment.Split('&').Select(_ => _.Split(new[] { '=' }, 2)).ToDictionary(_ => _[0], _ => _.Length == 2 ? _[1] : string.Empty, StringComparer.OrdinalIgnoreCase);

            string accessToken;
            if (fragments.TryGetValue("access_token", out accessToken))
            {
                token = OAuthToken.Save(accessToken);
            }

            return token;
        }
    }
}
