using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace VideoApi.Telemetry
{
    public class SuccessfulDependencyProcessor : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }
        public SuccessfulDependencyProcessor(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;
            
            if (dependency?.ResultCode.Equals("404", StringComparison.OrdinalIgnoreCase) == true)
            {
                // To filter out external 404 errors.
                return;
            }

            this.Next.Process(item);
        }
    }
}
