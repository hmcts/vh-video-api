using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VideoApi.Services.Contracts;

namespace VideoApi.Health;

public class WowzaHealthCheck : IHealthCheck
{
    private readonly IAudioPlatformService _audioPlatformService;

    public WowzaHealthCheck(IAudioPlatformService audioPlatformService)
    {
        _audioPlatformService = audioPlatformService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var healthyResult = await _audioPlatformService.GetDiagnosticsAsync();
            return healthyResult ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded("Wowza is not healthy");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Degraded(exception.Message, exception);
        }
    }
}
