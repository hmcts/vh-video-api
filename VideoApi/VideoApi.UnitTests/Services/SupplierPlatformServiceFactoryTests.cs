using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Stub;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Enums;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.SupplierStub;
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
            var logger = new Mock<ILogger<SupplierPlatformService>>();
            var pollyRetryService = new Mock<IPollyRetryService>();
            featureToggles.Setup(x => x.SendTransferRolesEnabled()).Returns(true);
            _serviceProvider.Setup(x => x.GetService(typeof(ILogger<SupplierPlatformService>))).Returns(logger.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IPollyRetryService))).Returns(pollyRetryService.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IFeatureToggles))).Returns(featureToggles.Object);
            SetUpVodafoneSupplier();
            SetUpStubSupplier();
        }
        
        [TestCase(Supplier.Vodafone)]
        [TestCase(Supplier.Stub)]
        public void Should_create_supplier_platform_service(Supplier supplier)
        {
            // Arrange
            var sut = new SupplierPlatformServiceFactory(_serviceProvider.Object);

            // Act
            var service = sut.Create((VideoApi.Domain.Enums.Supplier)supplier);

            // Assert
            service.Should().BeOfType<SupplierPlatformService>();
            service.Should().NotBeNull();

            switch (supplier)
            {
                case Supplier.Vodafone:
                    service.GetHttpClient().Should().BeAssignableTo<IVodafoneApiClient>();
                    service.GetSupplierConfiguration().Should().BeAssignableTo<VodafoneConfiguration>();
                    break;
                case Supplier.Stub:
                    service.GetHttpClient().Should().BeAssignableTo<ISupplierStubClient>();
                    service.GetSupplierConfiguration().Should().BeAssignableTo<SupplierStubConfiguration>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(supplier), supplier, null);
            }
        }
    
        private void SetUpVodafoneSupplier()
        {
            SetUpSupplier<IVodafoneApiClient, VodafoneConfiguration>(typeof(IVodafoneApiClient), typeof(VodafoneConfiguration));
        }

        private void SetUpStubSupplier()
        {
            SetUpSupplier<ISupplierStubClient, SupplierStubConfiguration>(typeof(ISupplierStubClient), typeof(SupplierStubConfiguration));
        }
        
        private void SetUpSupplier<TApiClient, TConfig>(Type apiClientType, Type configType)
            where TApiClient : class
            where TConfig : SupplierConfiguration, new()
        {
            var apiClientMock = new Mock<TApiClient>();
            var config = new TConfig();
            var configOptionsMock = new Mock<IOptions<TConfig>>();
            configOptionsMock.Setup(m => m.Value).Returns(config);

            _serviceProvider.Setup(x => x.GetService(apiClientType)).Returns(apiClientMock.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IOptions<>).MakeGenericType(configType))).Returns(configOptionsMock.Object);
        }
    }
}
