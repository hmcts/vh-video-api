using Microsoft.Extensions.DependencyInjection;
using VideoApi.DAL;

namespace VideoApi.Health;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddVhHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<VideoApiDbContext>("Database VhBookings")
            .AddCheck<KinlyApiHealthCheck>("Kinly API");
            
        return services;
    }
}
