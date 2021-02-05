using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Newtonsoft.Json;

namespace VideoApi.Common
{
    /// <summary>
    /// The application logger class send telemetry to Application Insights.
    /// </summary>
    public static class ApplicationLogger
    {
        private static readonly TelemetryClient TelemetryClient = InitTelemetryClient();
        
        private static TelemetryClient InitTelemetryClient() {
            var config = TelemetryConfiguration.CreateDefault();
            var client = new TelemetryClient(config);
            return client;
        }

        public static void Trace(string traceCategory, string eventTitle, string information)
        {
            var telematryTrace = new TraceTelemetry(traceCategory, severityLevel: SeverityLevel.Information);
            telematryTrace.Properties.Add("Information", information);
            telematryTrace.Properties.Add("Event", eventTitle);
            TelemetryClient.TrackTrace(telematryTrace);
        }

        public static void TraceWithProperties(string traceCategory, string eventTitle, string user, IDictionary<string, string> properties)
        {
            var telematryTrace = new TraceTelemetry(traceCategory.ToString(), severityLevel: SeverityLevel.Information);

            telematryTrace.Properties.Add("Event", eventTitle);

            telematryTrace.Properties.Add("User", user);

            if (properties != null)
            {
                foreach (KeyValuePair<string, string> entry in properties)
                {
                    telematryTrace.Properties.Add(entry.Key, entry.Value);
                }
            }

            TelemetryClient.TrackTrace(telematryTrace);
          
        }
   
        public static void TraceWithProperties(string traceCategory, string eventTitle, string user)
        {
            TraceWithProperties(traceCategory, eventTitle, user, null);
        }

        public static void TraceWithObject(string traceCategory, string eventTitle, string user, object valueToSerialized)
        {
            var telematryTrace = new TraceTelemetry(traceCategory.ToString(), severityLevel: SeverityLevel.Information);

            telematryTrace.Properties.Add("Event", eventTitle);

            telematryTrace.Properties.Add("User", user);

            if (valueToSerialized != null)
            {
                telematryTrace.Properties.Add(valueToSerialized.GetType().Name, JsonConvert.SerializeObject(valueToSerialized, Formatting.None));
            }

            TelemetryClient.TrackTrace(telematryTrace);
        }

        public static void TraceWithObject(string traceCategory, string eventTitle, string user)
        {
            TraceWithObject(traceCategory, eventTitle, user, null);
        }

        public static void TraceException(string traceCategory, string eventTitle, Exception exception, string user, IDictionary<string, string> properties)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var telematryException = new ExceptionTelemetry(exception);

            telematryException.Properties.Add("Event", traceCategory + " " + eventTitle);

            if (!string.IsNullOrEmpty(user))
            {
                telematryException.Properties.Add("User", user);
            }

            if (properties != null)
            {
                foreach (KeyValuePair<string, string> entry in properties)
                {
                    telematryException.Properties.Add(entry.Key, entry.Value);
                }
            }

            TelemetryClient.TrackException(telematryException);
        }

        public static void TraceException(string traceCategory, string eventTitle, Exception exception, string user)
        {
            TraceException(traceCategory, eventTitle, exception, user, null);
        }

        public static void TraceEvent(string eventTitle, IDictionary<string, string> properties)
        {
            var telemetryEvent = new EventTelemetry(eventTitle);

            if (properties != null)
            {
                foreach (KeyValuePair<string, string> entry in properties)
                {
                    telemetryEvent.Properties.Add(entry.Key, entry.Value);
                }
            }

            TelemetryClient.TrackEvent(telemetryEvent);
        }

        public static void TraceRequest(string operationName, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
        {
            var telemetryOperation = new RequestTelemetry(operationName, startTime, duration, responseCode, success);
            TelemetryClient.TrackRequest(telemetryOperation);
        }
    }
}
