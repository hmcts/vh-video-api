namespace VideoApi.Services.Clients.Models;

public class HealthCheckResponse
{
    [JsonPropertyName("health_status")]
    public PlatformHealth? HealthStatus { get; set; }
}

public enum PlatformHealth
{
    [System.Runtime.Serialization.EnumMember(Value = @"HEALTHY")]
    Healthy = 0,

    [System.Runtime.Serialization.EnumMember(Value = @"UNHEALTHY")]
    Unhealthy = 1,
}
