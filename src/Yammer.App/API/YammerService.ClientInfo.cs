//---------------------------------------------------------------------
// <copyright file="YammerService.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.API
{
    using System;

    /// <summary>
    /// Yammer service class
    /// </summary>
    public partial class YammerService
    {
        /// <summary>
        /// The client information
        /// TODO: Register you application and get a client id, https://developer.yammer.com/docs/app-registration
        /// </summary>
        public static readonly OAuthClientInfo ClientInfo = new OAuthClientInfo(clientId: "", callback: new Uri("yam://auth"));
    }
}
