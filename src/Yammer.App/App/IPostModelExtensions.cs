//---------------------------------------------------------------------
// <copyright file="IPostModelExtensions.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Threading.Tasks;
    using Yammer.API;
    using Yammer.API.Models;

    /// <summary>
    /// Post model interface extensions
    /// </summary>
    public static class IPostModelExtensions
    {
        /// <summary>
        /// Posts the message, asynchronously.
        /// </summary>
        /// <returns>
        /// The completion task.
        /// </returns>
        public static async Task PostMessageAsync(this IPostModel model)
        {
            model.ThrowIfNull(nameof(model));
            string messageText = model.ReplyMessage;
            model.ReplyMessage = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(messageText))
                {
                    PostRequest request = new PostRequest(messageText);
                    if (model.GroupId.HasValue)
                    {
                        request.GroupId = model.GroupId;
                    }

                    if (model.ReplyTo != null)
                    {
                        request.ReplyToId = model.ReplyTo.Id;
                    }

                    System.Diagnostics.Debug.Assert(request.GroupId.HasValue || request.ReplyToId.HasValue);

                    MessageFeed feed = await YammerService.Instance.PostMessageAsync(request);

                    System.Diagnostics.Debug.Assert(feed.Messages.Length == 1);
                    model.MessagePosted(feed);
                }
            }
            catch
            {
                model.ReplyMessage = messageText;
            }
        }
    }
}
