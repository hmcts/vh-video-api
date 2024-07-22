using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;

namespace VideoApi.Services.Handlers;

public class SupplierLoggingDelegatingHandler(
    ILogger<SupplierLoggingDelegatingHandler> logger,
    TelemetryClient telemetryClient)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestTelemetry = new RequestTelemetry
        {
            Name = $"HTTP {request.Method} {request.RequestUri}",
            Url = request.RequestUri,
            Timestamp = DateTimeOffset.Now
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Log request body if it exists
        if (request.Content != null)
        {
            var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            requestTelemetry.Properties.Add("SupplierApiRequestBody", requestBody);
            logger.LogInformation("Request to {RequestUri}: {RequestBody}", request.RequestUri, requestBody);
        }

        // Proceed with the request
        var response = await base.SendAsync(request, cancellationToken);

        stopwatch.Stop();
        requestTelemetry.Duration = stopwatch.Elapsed;
        requestTelemetry.ResponseCode = response.StatusCode.ToString();
        requestTelemetry.Success = response.IsSuccessStatusCode;
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        requestTelemetry.Properties.Add("SupplierApiResponseBody", responseBody);

        telemetryClient.TrackRequest(requestTelemetry);

        return response;
    }
}
