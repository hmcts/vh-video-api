using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;

namespace VideoApi.Health;

public class SupplierApiHealthCheck : IHealthCheck
{
    private readonly IVideoPlatformService _videoPlatformService;

    public SupplierApiHealthCheck(IVideoPlatformService videoPlatformService)
    {
        _videoPlatformService = videoPlatformService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        try
        {
            var result =  await _videoPlatformService.GetPlatformHealthAsync();
            return result.Health_status == PlatformHealth.HEALTHY ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(exception.Message, exception);
        }
    }
}
