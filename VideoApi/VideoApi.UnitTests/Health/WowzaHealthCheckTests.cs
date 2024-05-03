using System;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using VideoApi.Health;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Health;

public class WowzaHealthCheckTests
{
    private AutoMock _mocker;
    private WowzaHealthCheck _sut;
    
    [SetUp]
    public void Setup()
    {
        _mocker = AutoMock.GetLoose();
        _sut = _mocker.Create<WowzaHealthCheck>();
    }
    
    [Test]
    public async Task Should_return_healthy_if_wowza_api_is_healthy()
    {
        _mocker.Mock<IAudioPlatformService>()
            .Setup(x => x.GetDiagnosticsAsync())
            .ReturnsAsync(true);
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Healthy);
    }
    
    [Test]
    public async Task Should_return_degraded_if_wowza_api_is_unhealthy()
    {
        _mocker.Mock<IAudioPlatformService>()
            .Setup(x => x.GetDiagnosticsAsync())
            .ReturnsAsync(false);
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Degraded);
    }
    
    [Test]
    public async Task Should_return_degraded_if_wowza_api_is_unavailable()
    {
        _mocker.Mock<IAudioPlatformService>()
            .Setup(x => x.GetDiagnosticsAsync())
            .ThrowsAsync(new InvalidOperationException("Test Failed"));
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Degraded);
    }
}
