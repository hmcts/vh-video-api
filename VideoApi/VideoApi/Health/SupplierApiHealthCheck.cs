using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VideoApi.Contract.Enums;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;

namespace VideoApi.Health;

public class SupplierApiHealthCheck : IHealthCheck
{
    private readonly ISupplierPlatformServiceFactory _supplierPlatformServiceFactory;

    public SupplierApiHealthCheck(ISupplierPlatformServiceFactory supplierPlatformServiceFactory)
    {
        _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        try
        {
            var videoPlatformService = _supplierPlatformServiceFactory.Create(Supplier.Kinly);
            var result =  await videoPlatformService.GetPlatformHealthAsync();
            return result.Health_status == PlatformHealth.HEALTHY ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(exception.Message, exception);
        }
    }
}
