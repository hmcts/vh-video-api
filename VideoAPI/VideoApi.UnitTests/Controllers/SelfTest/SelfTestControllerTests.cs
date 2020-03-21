using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Net;
using Video.API.Controllers;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;

namespace VideoApi.UnitTests.Controllers.SelfTest
{
    public class SelfTestControllerTests
    {
        private SelfTestController _controller;
        private Mock<ILogger<SelfTestController>> _mockLogger;
        private ServicesConfiguration _servicesConfiguration;


        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<SelfTestController>>();
        }

        [Test]
        public void Should_return_okay_with_response()
        {
            _servicesConfiguration = new ServicesConfiguration();
            _controller = new SelfTestController(Options.Create(_servicesConfiguration), _mockLogger.Object);
            _servicesConfiguration.PexipSelfTestNode = "test.self-test.node";

            var response = (OkObjectResult)_controller.GetPexipServicesConfiguration();
            response.Should().NotBeNull();
            var pexipConfig = (PexipConfigResponse)response.Value;
            pexipConfig.PexipSelfTestNode.Should().Be(_servicesConfiguration.PexipSelfTestNode);
        }

        [Test]
        public void Should_return_not_found()
        {
            _servicesConfiguration = null;
            _controller = new SelfTestController(Options.Create(_servicesConfiguration), _mockLogger.Object);

            var response = (NotFoundResult)_controller.GetPexipServicesConfiguration();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
