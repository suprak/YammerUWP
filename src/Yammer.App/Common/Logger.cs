//---------------------------------------------------------------------
// <copyright file="Logger.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Logger class
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// The internal instance
        /// </summary>
        private static ILogger InternalInstance = new Logger();

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        protected Logger()
        {
        }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public static ILogger Instance
        {
            get { return Logger.InternalInstance; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                Logger.InternalInstance = value;
            }
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public virtual void LogException(Exception exception)
        {
            Debug.WriteLine(string.Concat(Logger.GetTimestamp(), " An exception occurred at: ", Environment.NewLine, exception));
        }

        /// <summary>
        /// Logs the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="metrics">The metrics.</param>
        public virtual void LogEvent(string name, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            Debug.WriteLine("{0} A '{1}' event occurred", Logger.GetTimestamp(), name);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void LogMessage(string message)
        {
            Debug.WriteLine("{0} {1}", Logger.GetTimestamp(), message);
        }

        /// <summary>
        /// Logs the metric.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="properties">The properties.</param>
        public virtual void LogMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            Debug.WriteLine("{0} '{1}' = {2}", Logger.GetTimestamp(), name, value);
        }

        /// <summary>
        /// Logs the page view.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="duration">The duration.</param>
        public virtual void LogPageView(string name, TimeSpan duration)
        {
            Debug.WriteLine("{0} '{1}' page viewed for '{2}' seconds.", Logger.GetTimestamp(), name, duration.TotalSeconds);
        }

        /// <summary>
        /// Gets the current time stamp.
        /// </summary>
        /// <returns>
        /// The current time stamp.
        /// </returns>
        protected static string GetTimestamp()
        {
            return string.Concat("[", DateTimeOffset.Now.ToString("G"), "]");
        }
    }
}
