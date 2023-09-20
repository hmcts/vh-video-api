using System;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NUnit.Framework;
using VideoApi.Health;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Health;

public class KinlyApiHealthCheckTests
{
    private AutoMock _mocker;
    private KinlyApiHealthCheck _sut;
    
    [SetUp]
    public void Setup()
    {
        _mocker = AutoMock.GetLoose();
        _sut = _mocker.Create<KinlyApiHealthCheck>();
    }
    
    [Test]
    public async Task Should_return_healthy_if_kinly_api_is_healthy()
    {
        _mocker.Mock<IVideoPlatformService>()
            .Setup(x => x.GetPlatformHealthAsync())
            .ReturnsAsync(new HealthCheckResponse
            {
                Health_status = PlatformHealth.HEALTHY
            });
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Healthy);
    }
    
    [Test]
    public async Task Should_return_unhealthy_if_kinly_api_is_unhealthy()
    {
        _mocker.Mock<IVideoPlatformService>()
            .Setup(x => x.GetPlatformHealthAsync())
            .ReturnsAsync(new HealthCheckResponse
            {
                Health_status = PlatformHealth.UNHEALTHY
            });
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
    
    [Test]
    public async Task Should_return_unhealthy_if_kinly_api_is_unavailable()
    {
        _mocker.Mock<IVideoPlatformService>()
            .Setup(x => x.GetPlatformHealthAsync())
            .ThrowsAsync(new InvalidOperationException("Test Failed"));
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
}
