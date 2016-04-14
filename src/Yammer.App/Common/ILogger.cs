//---------------------------------------------------------------------
// <copyright file="ILogger.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void LogException(Exception exception);

        /// <summary>
        /// Logs the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="metrics">The metrics.</param>
        void LogEvent(string name, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogMessage(string message);

        /// <summary>
        /// Logs the metric.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="properties">The properties.</param>
        void LogMetric(string name, double value, IDictionary<string, string> properties = null);

        /// <summary>
        /// Logs the page view.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="duration">The duration.</param>
        void LogPageView(string name, TimeSpan duration);
    }
}
