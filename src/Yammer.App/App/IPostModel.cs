//---------------------------------------------------------------------
// <copyright file="IPostModel.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System.Collections.Generic;
    using Yammer.API.Models;

    /// <summary>
    /// Post model class
    /// </summary>
    public interface IPostModel
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        IList<Message> Messages
        {
            get;
        }

        /// <summary>
        /// Gets or sets the reply message.
        /// </summary>
        /// <value>
        /// The reply message.
        /// </value>
        string ReplyMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the group identifier.
        /// </summary>
        /// <value>
        /// The group identifier.
        /// </value>
        int? GroupId
        {
            get;
        }

        /// <summary>
        /// Gets the reply to.
        /// </summary>
        /// <value>
        /// The reply to.
        /// </value>
        Message ReplyTo
        {
            get;
        }

        /// <summary>
        /// Message posted update.
        /// </summary>
        /// <param name="responseFeed">The response feed.</param>
        void MessagePosted(MessageFeed responseFeed);
    }
}
