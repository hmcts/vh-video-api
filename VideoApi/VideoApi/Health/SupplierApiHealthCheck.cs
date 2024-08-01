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
    private readonly IFeatureToggles _featureToggles;

    public SupplierApiHealthCheck(ISupplierPlatformServiceFactory supplierPlatformServiceFactory, 
        IFeatureToggles featureToggles)
    {
        _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
        _featureToggles = featureToggles;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new ())
    {
        try
        {
            var vodafoneEnabled = _featureToggles.VodafoneIntegrationEnabled();
            var supplier = vodafoneEnabled ? Domain.Enums.Supplier.Vodafone : Domain.Enums.Supplier.Kinly;
            var videoPlatformService = _supplierPlatformServiceFactory.Create(supplier);
            var result =  await videoPlatformService.GetPlatformHealthAsync();
            return result.Health_status == PlatformHealth.HEALTHY ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(exception.Message, exception);
        }
    }
}
