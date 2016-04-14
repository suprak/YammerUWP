//---------------------------------------------------------------------
// <copyright file="CollectionVisibilityConverter.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using Windows.UI.Xaml;

    /// <summary>
    /// Collection visibility converter class
    /// </summary>
    public class CollectionVisibilityConverter : ValueConverterBase
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
            Array array = (Array)value;
            return array == null || array.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
