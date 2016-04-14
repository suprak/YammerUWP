//---------------------------------------------------------------------
// <copyright file="MessageExtensions.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Yammer.API;
    using Yammer.API.Models;

    /// <summary>
    /// Message extensions class
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Toggles the like state, asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="currentUser">The current user.</param>
        /// <returns>
        /// The completion task.
        /// </returns>
        public static async Task ToggleLikeAsync(this Message message)
        {
            User currentUser = await YammerService.Instance.GetCurrentUserAsync();
            LikedBy.User likedByUser = message.LikedBy.Names.Where(_ => _.UserId == currentUser.Id).FirstOrDefault();
            if (likedByUser != null)
            {
                await YammerService.Instance.UnLikeMessageAsync(message.Id);
                message.LikedBy.Names.Remove(likedByUser);
                message.IsLiked = false;
            }
            else
            {
                likedByUser = new LikedBy.User()
                {
                    FullName = currentUser.FullName,
                    NetworkId = currentUser.NetworkId,
                    UserId = currentUser.Id
                };

                await YammerService.Instance.LikeMessageAsync(message.Id);
                message.LikedBy.Names.Add(likedByUser);
                message.IsLiked = true;
            }

            message.TriggerPropertyChanged(null);
        }
    }
}
