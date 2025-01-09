using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Controllers.SelfTest
{
    public class SelfTestControllerTests
    {
        private SelfTestController _controller;
        private Mock<ILogger<SelfTestController>> _mockLogger;
        private Mock<IVideoPlatformService> _supplierPlatformService;
        private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactory;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<SelfTestController>>();
            _supplierPlatformService = new Mock<IVideoPlatformService>();
            _supplierPlatformServiceFactory = new Mock<ISupplierPlatformServiceFactory>();
            _supplierPlatformServiceFactory.Setup(x => x.Create(It.IsAny<VideoApi.Domain.Enums.Supplier>())).Returns(_supplierPlatformService.Object);
        }

        [Test]
        public void Should_return_okay_with_response()
        {
            const VideoApi.Domain.Enums.Supplier supplier = VideoApi.Domain.Enums.Supplier.Vodafone;
            var supplierConfiguration = new VodafoneConfiguration
            {
                PexipSelfTestNode = "VodafonePexipSelfTestNode"
            };

            _supplierPlatformService.Setup(x => x.GetSupplierConfiguration()).Returns(supplierConfiguration);
            _supplierPlatformServiceFactory.Setup(x => x.Create(supplier)).Returns(_supplierPlatformService.Object);
            _controller = new SelfTestController(_supplierPlatformServiceFactory.Object, _mockLogger.Object);

            var response = (OkObjectResult)_controller.GetPexipServicesConfiguration();
            response.Should().NotBeNull();
            var pexipConfig = (PexipConfigResponse)response.Value;
            pexipConfig.PexipSelfTestNode.Should().Be(supplierConfiguration.PexipSelfTestNode);
            VerifySupplierUsed(supplier, Times.Exactly(1));
        }

        [Test]
        public void Should_return_not_found()
        {
            _supplierPlatformService.Setup(x => x.GetSupplierConfiguration()).Returns((SupplierConfiguration)null);
            // _supplierPlatformServiceFactory.Setup(x => x.Create(VideoApi.Domain.Enums.Supplier.Kinly)).Returns(_supplierPlatformService.Object);
            _controller = new SelfTestController(_supplierPlatformServiceFactory.Object, _mockLogger.Object);

            var response = (NotFoundResult)_controller.GetPexipServicesConfiguration();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        private void VerifySupplierUsed(VideoApi.Domain.Enums.Supplier supplier, Times times)
        {
            _supplierPlatformServiceFactory.Verify(x => x.Create(supplier), times);
        }
    }
}
