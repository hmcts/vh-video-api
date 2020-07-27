using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Video.API.Controllers;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class ConferenceManagementControllerTestBase
    {
        protected ConferenceManagementController Controller;
        protected Mock<ILogger<ConferenceManagementController>> MockLogger;
        protected Mock<IVideoPlatformService> VideoPlatformServiceMock;
        
        [SetUp]
        public void Setup()
        {
            MockLogger = new Mock<ILogger<ConferenceManagementController>>();
            VideoPlatformServiceMock = new Mock<IVideoPlatformService>();
            
            Controller = new ConferenceManagementController(VideoPlatformServiceMock.Object, MockLogger.Object);
        }
    }
    
    public class StartVideoHearingTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_return_accepted_when_start_hearing_has_been_requested()
        {
            var conferenceId = Guid.NewGuid();
            
            var result = await Controller.StartVideoHearingAsync(conferenceId);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(x => x.StartHearingAsync(conferenceId), Times.Once);
        }

        [Test] public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "You're not allowed to start this hearing";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new KinlyApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.StartHearingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId);
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }
    }
}
