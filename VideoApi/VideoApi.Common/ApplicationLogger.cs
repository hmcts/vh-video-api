using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace VideoApi.Common
{
    /// <summary>
    /// The application logger class sends telemetry to Application Insights.
    /// </summary>
    public static class ApplicationLogger
    {
        private static readonly TelemetryClient TelemetryClient = InitTelemetryClient();

        private static TelemetryClient InitTelemetryClient() {
            var config = TelemetryConfiguration.CreateDefault();
            var client = new TelemetryClient(config);
            return client;
        }

        public static void TraceException(string traceCategory, string eventTitle, Exception exception, IPrincipal user, IDictionary<string, string> properties)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var telemetryException = new ExceptionTelemetry(exception);

            telemetryException.Properties.Add("Event", traceCategory + " " + eventTitle);

            if (user?.Identity != null)
            {
                telemetryException.Properties.Add("User", user.Identity.Name);
            }

            if (properties != null)
            {
                foreach (var entry in properties)
                {
                    telemetryException.Properties.Add(entry.Key, entry.Value);
                }
            }

            TelemetryClient.TrackException(telemetryException);
        }
    }
}
