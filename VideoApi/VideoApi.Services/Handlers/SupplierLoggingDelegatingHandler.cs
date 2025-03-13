using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VideoApi.Services.Handlers;

public class SupplierLoggingDelegatingHandler(ILogger<SupplierLoggingDelegatingHandler> logger) : DelegatingHandler
{
    private static readonly ActivitySource ActivitySource = new("SupplierLoggingDelegatingHandler");
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("SendToSupplier", ActivityKind.Client);
        activity?.SetTag("http.method", request.Method.ToString());
        activity?.SetTag("http.url", request.RequestUri?.ToString());
        
        // Log request body if present
        if (request.Content != null)
        {
            var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            activity?.SetTag("http.request.body", requestBody);
            logger.LogInformation("Request to {RequestUri}: {RequestBody}", request.RequestUri, requestBody);
        }
        
        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();
        
        activity?.SetTag("http.status_code", (int)response.StatusCode);
        activity?.SetTag("http.success", response.IsSuccessStatusCode);
        activity?.SetTag("duration_ms", stopwatch.ElapsedMilliseconds);
        
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        activity?.SetTag("http.response.body", responseBody);
        
        return response;
    }
}
