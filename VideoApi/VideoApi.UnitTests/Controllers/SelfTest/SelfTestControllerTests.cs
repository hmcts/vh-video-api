using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Contract.Enums;
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
        private SupplierConfiguration _kinlyConfiguration;
        private Mock<IVideoPlatformService> _supplierPlatformService;
        private Mock<ISupplierPlatformServiceFactory> _supplierPlatformServiceFactory;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<SelfTestController>>();
            _supplierPlatformService = new Mock<IVideoPlatformService>();
            _supplierPlatformServiceFactory = new Mock<ISupplierPlatformServiceFactory>();
        }

        [Test]
        public void Should_return_okay_with_response()
        {
            _kinlyConfiguration = new KinlyConfiguration();
            _supplierPlatformService.Setup(x => x.GetSupplierConfiguration()).Returns(_kinlyConfiguration);
            _supplierPlatformServiceFactory.Setup(x => x.Create(Supplier.Kinly)).Returns(_supplierPlatformService.Object);
            _controller = new SelfTestController(_supplierPlatformServiceFactory.Object, _mockLogger.Object);
            _kinlyConfiguration.PexipSelfTestNode = "test.self-test.node";

            var response = (OkObjectResult)_controller.GetPexipServicesConfiguration();
            response.Should().NotBeNull();
            var pexipConfig = (PexipConfigResponse)response.Value;
            pexipConfig.PexipSelfTestNode.Should().Be(_kinlyConfiguration.PexipSelfTestNode);
        }

        [Test]
        public void Should_return_not_found()
        {
            _kinlyConfiguration = null;
            _supplierPlatformService.Setup(x => x.GetSupplierConfiguration()).Returns(_kinlyConfiguration);
            _supplierPlatformServiceFactory.Setup(x => x.Create(Supplier.Kinly)).Returns(_supplierPlatformService.Object);
            _controller = new SelfTestController(_supplierPlatformServiceFactory.Object, _mockLogger.Object);

            var response = (NotFoundResult)_controller.GetPexipServicesConfiguration();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
