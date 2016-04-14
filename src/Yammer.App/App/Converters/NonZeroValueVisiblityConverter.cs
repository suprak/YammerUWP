//---------------------------------------------------------------------
// <copyright file="NonZeroValueVisiblityConverter.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using Windows.UI.Xaml;

    /// <summary>
    /// Non zero value visiblity converter class
    /// </summary>
    public class NonZeroValueVisiblityConverter : ValueConverterBase
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
            int intValue = (int)value;
            return intValue == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
