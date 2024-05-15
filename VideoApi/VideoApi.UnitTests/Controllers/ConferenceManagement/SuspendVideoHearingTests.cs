using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VideoApi.Services.Clients;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class SuspendVideoHearingTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_return_accepted_when_suspend_hearing_has_been_requested()
        {
            var conferenceId = Guid.NewGuid();
            
            var result = await Controller.SuspendHearingAsync(conferenceId);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(x => x.SuspendHearingAsync(conferenceId), Times.Once);
        }

        [Test] public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "You're not allowed to suspend this hearing";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new SupplierApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.SuspendHearingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.SuspendHearingAsync(conferenceId);
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }
    }
}
