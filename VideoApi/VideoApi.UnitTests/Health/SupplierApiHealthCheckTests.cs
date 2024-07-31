using System;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Health;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Health;

public class SupplierApiHealthCheckTests
{
    private AutoMock _mocker;
    private SupplierApiHealthCheck _sut;
    
    [SetUp]
    public void Setup()
    {
        _mocker = AutoMock.GetLoose();
        _sut = _mocker.Create<SupplierApiHealthCheck>();
    }
    
    [Test]
    public async Task Should_return_healthy_if_kinly_api_is_healthy()
    {
        var videoPlatformServiceMock = _mocker.Mock<IVideoPlatformService>();
        videoPlatformServiceMock
            .Setup(x => x.GetPlatformHealthAsync())
            .ReturnsAsync(new HealthCheckResponse
            {
                Health_status = PlatformHealth.HEALTHY
            });
        _mocker.Mock<ISupplierPlatformServiceFactory>()
            .Setup(x => x.Create(It.IsAny<VideoApi.Domain.Enums.Supplier>())).Returns(videoPlatformServiceMock.Object);
        
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
