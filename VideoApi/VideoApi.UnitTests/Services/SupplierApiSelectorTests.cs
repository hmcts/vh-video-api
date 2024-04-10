using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services;
using VideoApi.Services.Clients;

namespace VideoApi.UnitTests.Services;

public class SupplierApiSelectorTests
{
    private Mock<IServiceProvider> _serviceProvider;
    private Mock<IFeatureToggles> _featureToggles;
    private IOptions<KinlyConfiguration> _kinlyConfigOptions;
    private IOptions<VodafoneConfiguration> _vodafoneConfigOptions;
    private Mock<IKinlyApiClient> _kinlyApiClient;
    private Mock<IVodafoneApiClient> _vodafoneClient;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = new Mock<IServiceProvider>();
        _featureToggles = new Mock<IFeatureToggles>();
        _kinlyApiClient = new Mock<IKinlyApiClient>();
        _vodafoneClient = new Mock<IVodafoneApiClient>();
        _serviceProvider.Setup(x => x.GetService<IKinlyApiClient>()).Returns(_kinlyApiClient.Object);
        _serviceProvider.Setup(x => x.GetService<IVodafoneApiClient>()).Returns(_vodafoneClient.Object);
        _kinlyConfigOptions = Options.Create(new KinlyConfiguration());
        _vodafoneConfigOptions = Options.Create(new VodafoneConfiguration());
    }
    
    [Test]
    public void Should_return_kinly_http_client_when_vodafone_integration_is_disabled()
    {
        // Arrange
        var sut = new SupplierApiSelector(_serviceProvider.Object, _featureToggles.Object, _kinlyConfigOptions, _vodafoneConfigOptions);
        _featureToggles.Setup(x => x.VodafoneIntegrationEnabled()).Returns(false);
        
        // Act
        var result = sut.GetHttpClient();
        
        // Assert
        result.Should().Be(_kinlyApiClient.Object);
    }
    
    [Test]
    public void Should_return_vodafone_http_client_when_vodafone_integration_is_enabled()
    {
        // Arrange
        var sut = new SupplierApiSelector(_serviceProvider.Object, _featureToggles.Object, _kinlyConfigOptions, _vodafoneConfigOptions);
        _featureToggles.Setup(x => x.VodafoneIntegrationEnabled()).Returns(true);
        
        // Act
        var result = sut.GetHttpClient();
        
        // Assert
        result.Should().Be(_vodafoneClient.Object);
    }
    
    [Test]
    public void Should_return_kinly_configuration_when_vodafone_integration_is_disabled()
    {
        // Arrange
        var sut = new SupplierApiSelector(_serviceProvider.Object, _featureToggles.Object, _kinlyConfigOptions, _vodafoneConfigOptions);
        _featureToggles.Setup(x => x.VodafoneIntegrationEnabled()).Returns(false);
        
        // Act
        var result = sut.GetSupplierConfiguration();
        
        // Assert
        result.Should().Be(_kinlyConfigOptions.Value);
    }
    
    [Test]
    public void Should_return_vodafone_configuration_when_vodafone_integration_is_enabled()
    {
        // Arrange
        var sut = new SupplierApiSelector(_serviceProvider.Object, _featureToggles.Object, _kinlyConfigOptions, _vodafoneConfigOptions);
        _featureToggles.Setup(x => x.VodafoneIntegrationEnabled()).Returns(true);
        
        // Act
        var result = sut.GetSupplierConfiguration();
        
        // Assert
        result.Should().Be(_vodafoneConfigOptions.Value);
    }
}
