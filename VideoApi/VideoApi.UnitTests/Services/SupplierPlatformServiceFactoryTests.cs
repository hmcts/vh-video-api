using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoApi.Common.Security.Supplier.Kinly;
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
            var logger = new Mock<ILogger<SupplierPlatformService>>();
            var kinlySelfTestHttpClient = new Mock<IKinlySelfTestHttpClient>();
            var vodafoneSelfTestHttpClient = new Mock<IVodafoneSelfTestHttpClient>();
            var pollyRetryService = new Mock<IPollyRetryService>();
            var kinlyApiClient = new Mock<IKinlyApiClient>();
            var vodafoneApiClient = new Mock<IVodafoneApiClient>();
            var kinlyConfig = new KinlyConfiguration();
            var vodafoneConfig = new VodafoneConfiguration();
            var kinlyConfigOptions = new Mock<IOptions<KinlyConfiguration>>();
            var vodafoneConfigOptions = new Mock<IOptions<VodafoneConfiguration>>();
            kinlyConfigOptions.Setup(m => m.Value).Returns(kinlyConfig);
            vodafoneConfigOptions.Setup(m => m.Value).Returns(vodafoneConfig);
            _serviceProvider.Setup(x => x.GetService(typeof(ILogger<SupplierPlatformService>))).Returns(logger.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IVodafoneSelfTestHttpClient))).Returns(vodafoneSelfTestHttpClient.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IKinlySelfTestHttpClient))).Returns(kinlySelfTestHttpClient.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IPollyRetryService))).Returns(pollyRetryService.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IKinlyApiClient))).Returns(kinlyApiClient.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IVodafoneApiClient))).Returns(vodafoneApiClient.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IOptions<KinlyConfiguration>))).Returns(kinlyConfigOptions.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IOptions<VodafoneConfiguration>))).Returns(vodafoneConfigOptions.Object);
        }
        
        [TestCase(Supplier.Kinly)]
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
            
            if (supplier == Supplier.Kinly)
            {
                service.GetHttpClient().Should().BeAssignableTo<IKinlyApiClient>();
                service.GetSupplierConfiguration().Should().BeAssignableTo<KinlyConfiguration>();
            }
            else if (supplier == Supplier.Vodafone)
            {
                service.GetHttpClient().Should().BeAssignableTo<IVodafoneApiClient>();
                service.GetSupplierConfiguration().Should().BeAssignableTo<VodafoneConfiguration>();
            }
        }
    }
}
