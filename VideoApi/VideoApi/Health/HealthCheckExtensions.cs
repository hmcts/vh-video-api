using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VideoApi.DAL;

namespace VideoApi.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddVhHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddDbContextCheck<VideoApiDbContext>(name: "Database VhBookings", tags: new[] {"startup", "readiness"})
            .AddCheck<KinlyApiHealthCheck>(name: "Kinly API", tags: new[] {"startup", "readiness"})
            .AddCheck<WowzaHealthCheck>(name: "Wowza VM", tags: new[] {"startup", "readiness"});
            
        return services;
    }
}
