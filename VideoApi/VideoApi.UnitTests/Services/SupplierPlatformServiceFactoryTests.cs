using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Enums;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Services
{
    public class SupplierPlatformServiceFactoryTests
    {
        private Mock<IServiceProvider> _serviceProvider;
        
        [SetUp]
        public void SetUp()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            var featureToggles = new Mock<IFeatureToggles>();
            var logger = new Mock<ILogger<SupplierPlatformService>>();;
            var pollyRetryService = new Mock<IPollyRetryService>();
            var vodafoneApiClient = new Mock<IVodafoneApiClient>();
            var vodafoneConfig = new VodafoneConfiguration();
            var vodafoneConfigOptions = new Mock<IOptions<VodafoneConfiguration>>();
            vodafoneConfigOptions.Setup(m => m.Value).Returns(vodafoneConfig);
            featureToggles.Setup(x => x.SendTransferRolesEnabled()).Returns(true);
            _serviceProvider.Setup(x => x.GetService(typeof(ILogger<SupplierPlatformService>))).Returns(logger.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IPollyRetryService))).Returns(pollyRetryService.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IVodafoneApiClient))).Returns(vodafoneApiClient.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IOptions<VodafoneConfiguration>))).Returns(vodafoneConfigOptions.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IFeatureToggles))).Returns(featureToggles.Object);
        }
        
        [TestCase(Supplier.Vodafone)]
        public void Should_create_supplier_platform_service(Supplier supplier)
        {
            // Arrange
            var sut = new SupplierPlatformServiceFactory(_serviceProvider.Object);

            // Act
            var service = sut.Create((VideoApi.Domain.Enums.Supplier)supplier);

            // Assert
            service.Should().BeOfType<SupplierPlatformService>();
            service.Should().NotBeNull();
            
            if (supplier == Supplier.Vodafone)
            {
                service.GetHttpClient().Should().BeAssignableTo<IVodafoneApiClient>();
                service.GetSupplierConfiguration().Should().BeAssignableTo<VodafoneConfiguration>();
            }
        }
    }
}
