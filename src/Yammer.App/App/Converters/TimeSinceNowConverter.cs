//---------------------------------------------------------------------
// <copyright file="TimeSinceNowConverter.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Time since now converter class
    /// </summary>
    public class TimeSinceNowConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// Time since now.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan span = (TimeSpan)value;
            string result;
            if (span.TotalMinutes < 2)
            {
                result = "1 minute ago";
            }
            else if (span.TotalHours < 1)
            {
                result = Math.Truncate(span.TotalMinutes) + " minutes ago";
            }
            else if (span.TotalHours < 2)
            {
                result = "1 hour ago";
            }
            else if (span.TotalDays < 1)
            {
                result = Math.Truncate(span.TotalHours) + " hours ago";
            }
            else if (span.TotalDays < 2)
            {
                result = "1 day ago";
            }
            else
            {
                result = Math.Truncate(span.TotalDays) + " days ago";
            }

            return result;
        }

        /// <summary>
        /// Converts the value back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// Nothing.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
