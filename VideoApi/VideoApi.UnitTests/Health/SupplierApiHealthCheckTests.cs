using System;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using VideoApi.Contract.Enums;
using VideoApi.Health;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using Supplier = VideoApi.Domain.Enums.Supplier;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Health;

public class SupplierApiHealthCheckTests
{
    private AutoMock _mocker;
    private SupplierApiHealthCheck _sut;
    private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactoryMock;
    private Mock<IFeatureToggles> _featureTogglesMock;
    
    [SetUp]
    public void Setup()
    {
        _mocker = AutoMock.GetLoose();
        //_sut = _mocker.Create<SupplierApiHealthCheck>();
        _supplierPlatformServiceFactoryMock = new Mock<ISupplierPlatformServiceFactory>();
        _featureTogglesMock = new Mock<IFeatureToggles>();
        _sut = new SupplierApiHealthCheck(_supplierPlatformServiceFactoryMock.Object, _featureTogglesMock.Object);
    }
    
    [TestCase(false)]
    [TestCase(true)]
    public async Task Should_return_healthy_if_supplier_api_is_healthy(bool vodafoneEnabled)
    {
        var healthCheckResponse = new HealthCheckResponse
        {
            Health_status = PlatformHealth.HEALTHY
        };
        var kinlyVideoPlatformServiceMock = new Mock<IVideoPlatformService>();
        kinlyVideoPlatformServiceMock
            .Setup(x => x.GetPlatformHealthAsync())
            .ReturnsAsync(healthCheckResponse);
        var vodafoneVideoPlatformServiceMock = new Mock<IVideoPlatformService>();
        vodafoneVideoPlatformServiceMock
            .Setup(x => x.GetPlatformHealthAsync())
            .ReturnsAsync(healthCheckResponse);
        _supplierPlatformServiceFactoryMock
            .Setup(x => x.Create(Supplier.Kinly)).Returns(kinlyVideoPlatformServiceMock.Object);
        _supplierPlatformServiceFactoryMock
            .Setup(x => x.Create(Supplier.Vodafone)).Returns(vodafoneVideoPlatformServiceMock.Object);
        _featureTogglesMock.Setup(x => x.VodafoneIntegrationEnabled()).Returns(vodafoneEnabled);
        var supplier = vodafoneEnabled ? Supplier.Vodafone : Supplier.Kinly;
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        
        result.Status.Should().Be(HealthStatus.Healthy);
        VerifySupplierUsed(supplier, Times.Once());
    }
    
    [Test]
    public async Task Should_return_unhealthy_if_supplier_api_is_unhealthy()
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
    public async Task Should_return_unhealthy_if_supplier_api_is_unavailable()
    {
        _mocker.Mock<IVideoPlatformService>()
            .Setup(x => x.GetPlatformHealthAsync())
            .ThrowsAsync(new InvalidOperationException("Test Failed"));
        
        var result = await _sut.CheckHealthAsync(new HealthCheckContext());
        result.Status.Should().Be(HealthStatus.Unhealthy);
    }
    
    private void VerifySupplierUsed(Supplier supplier, Times times)
    {
        _supplierPlatformServiceFactoryMock.Verify(x => x.Create(supplier), times);
    }
}
