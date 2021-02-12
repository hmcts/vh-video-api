using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Net;
using VideoApi.Common.Security.Kinly;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;

namespace VideoApi.UnitTests.Controllers.SelfTest
{
    public class SelfTestControllerTests
    {
        private SelfTestController _controller;
        private Mock<ILogger<SelfTestController>> _mockLogger;
        private KinlyConfiguration _kinlyConfiguration;


        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<SelfTestController>>();
        }

        [Test]
        public void Should_return_okay_with_response()
        {
            _kinlyConfiguration = new KinlyConfiguration();
            _controller = new SelfTestController(Options.Create(_kinlyConfiguration), _mockLogger.Object);
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
            _controller = new SelfTestController(Options.Create(_kinlyConfiguration), _mockLogger.Object);

            var response = (NotFoundResult)_controller.GetPexipServicesConfiguration();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
