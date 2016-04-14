//---------------------------------------------------------------------
// <copyright file="TelemetryLogger.cs">
//  Copyright (c) 2016 Alex Karpus. MIT License (MIT)
// </copyright>
//---------------------------------------------------------------------

namespace Yammer.App
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Telemetry logger class
    /// </summary>
    public class TelemetryLogger : Logger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryLogger"/> class.
        /// </summary>
        public TelemetryLogger()
        {
        }

        /// <summary>
        /// Logs the event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="metrics">The metrics.</param>
        public override void LogEvent(string name, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            base.LogEvent(name);
            TelemetryClient client = new TelemetryClient();
            client.TrackEvent(name, properties, metrics);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public override void LogException(Exception exception)
        {
            base.LogException(exception);
            TelemetryClient client = new TelemetryClient();
            client.TrackException(exception);
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void LogMessage(string message)
        {
            base.LogMessage(message);
            TelemetryClient client = new TelemetryClient();
            client.TrackTrace(message);
        }

        /// <summary>
        /// Logs the metric.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="properties">The properties.</param>
        public override void LogMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            base.LogMetric(name, value, properties);
            TelemetryClient client = new TelemetryClient();
            client.TrackMetric(name, value, properties);
        }

        /// <summary>
        /// Logs the page view.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="duration">The duration.</param>
        public override void LogPageView(string name, TimeSpan duration)
        {
            base.LogPageView(name, duration);
            TelemetryClient client = new TelemetryClient();
            client.TrackPageView(new PageViewTelemetry(name) { Duration = duration });
        }
    }
}
