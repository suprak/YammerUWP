//---------------------------------------------------------------------
// <copyright file="CollectionLengthConverter.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;

    /// <summary>
    /// Collection length converter class
    /// </summary>
    public class CollectionLengthConverter : ValueConverterBase
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The array length.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Array)value)?.Length.ToString();
        }
    }
}
