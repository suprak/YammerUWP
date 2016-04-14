//---------------------------------------------------------------------
// <copyright file="Extensions.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Extensions class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Throws if string parameter is empty.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <returns>
        /// The string value.
        /// </returns>
        public static string ThrowIfEmpty(this string str, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException("argumentName");
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(argumentName);
            }

            return str;
        }

        /// <summary>
        /// Throws if null for generic parameters.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <returns>
        /// The value passed in.
        /// </returns>
        public static TValue ThrowIfNull<TValue>(this TValue value, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException("argumentName");
            }

            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return value;
        }

        /// <summary>
        /// Throws if collection is empty.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <returns>The collection passed in.</returns>
        public static IEnumerable<TValue> ThrowIfEmpty<TValue>(this IEnumerable<TValue> collection, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException("argumentName");
            }

            if (collection == null || !collection.Any())
            {
                throw new ArgumentNullException(argumentName);
            }

            return collection;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        /// <param name="frame">The frame.</param>
        public static void ClearCache(this Frame frame)
        {
            frame.ThrowIfNull(nameof(frame));
            int cacheSize = frame.CacheSize;
            frame.CacheSize = 0;
            frame.CacheSize = cacheSize;
        }

        /// <summary>
        /// Clears the history.
        /// </summary>
        /// <param name="frame">The frame.</param>
        public static void ClearHistory(this Frame frame)
        {
            frame.ThrowIfNull(nameof(frame));
            frame.BackStack.Clear();
            frame.ForwardStack.Clear();
            frame.ClearCache();
        }
    }
}
