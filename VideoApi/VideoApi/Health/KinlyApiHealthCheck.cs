using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VideoApi.Services.Contracts;

namespace VideoApi.Health;

public class KinlyApiHealthCheck : IHealthCheck
{
    private readonly IVideoPlatformService _videoPlatformService;

    public KinlyApiHealthCheck(IVideoPlatformService videoPlatformService)
    {
        _videoPlatformService = videoPlatformService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        try
        {
            await _videoPlatformService.GetPlatformHealthAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(exception.Message, exception);
        }
    }
}
