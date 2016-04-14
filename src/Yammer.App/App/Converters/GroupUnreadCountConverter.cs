//---------------------------------------------------------------------
// <copyright file="GroupUnreadCountConverter.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;

    /// <summary>
    /// Group unread count value converter class
    /// </summary>
    public class GroupUnreadCountConverter : ValueConverterBase
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            int count = (int)value;
            string result = string.Empty;
            if (count > 0 && count < 20)
            {
                result = count.ToString();
            }
            else if (count >= 20)
            {
                result = "20+";
            }

            return result;
        }
    }
}
