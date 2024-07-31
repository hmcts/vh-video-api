using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Extensions;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class PauseVideoHearingTests : ConferenceManagementControllerTestBase
    {
        [TestCase(Supplier.Kinly)]
        [TestCase(Supplier.Vodafone)]
        public async Task should_return_accepted_when_pause_hearing_has_been_requested(Supplier supplier)
        {
            var conferenceId = Guid.NewGuid();
            TestConference.SetSupplier(supplier);
            
            var result = await Controller.PauseVideoHearingAsync(conferenceId);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(x => x.PauseHearingAsync(conferenceId), Times.Once);
            VerifySupplierUsed(supplier, Times.Exactly(1));
        }

        [Test] public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "You're not allowed to pause this hearing";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new SupplierApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.PauseHearingAsync(It.IsAny<Guid>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.PauseVideoHearingAsync(conferenceId);
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }
    }
}
