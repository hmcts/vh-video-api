using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Services.Kinly;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class RequestTechnicalAssistanceTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_return_accepted_when_technical_assistance_has_been_requested()
        {
            var conferenceId = Guid.NewGuid();
            
            var result = await Controller.RequestTechnicalAssistanceAsync(conferenceId);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(x => x.RequestTechnicalAssistanceAsync(conferenceId), Times.Once);
        }

        [Test] public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "Unable to request technical assistance";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new KinlyApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.RequestTechnicalAssistanceAsync(It.IsAny<Guid>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.RequestTechnicalAssistanceAsync(conferenceId);
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }
    }
}
