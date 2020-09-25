using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Services.Kinly;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class StartVideoHearingTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_return_accepted_when_start_hearing_has_been_requested()
        {
            var conferenceId = Guid.NewGuid();
            var request = new StartHearingRequest
            {
                Layout = HearingLayout.OnePlus7
            };
            var result = await Controller.StartVideoHearingAsync(conferenceId, request);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(x => x.StartHearingAsync(conferenceId, Layout.ONE_PLUS_SEVEN), Times.Once);
        }

        [Test] public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "You're not allowed to start this hearing";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new KinlyApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.StartHearingAsync(It.IsAny<Guid>(), It.IsAny<Layout>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId, new StartHearingRequest());
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }
    }
}
