using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VideoApi.DAL;

namespace VideoApi.Health;

public static class HealthCheckExtensions
{
    private static readonly string[] HealthCheckTags = ["startup", "readiness"];
    
    public static IServiceCollection AddVhHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDbContextCheck<VideoApiDbContext>(name: "Database VhBookings", tags: HealthCheckTags)
            .AddCheck<SupplierApiHealthCheck>(name: "Supplier API", tags: HealthCheckTags)
            .AddCheck<WowzaHealthCheck>(name: "Wowza VM", tags: HealthCheckTags, failureStatus: HealthStatus.Degraded);
            
        return services;
    }
}
