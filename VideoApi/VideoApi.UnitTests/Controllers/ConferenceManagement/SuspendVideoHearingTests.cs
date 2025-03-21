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
            VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
        }
    }
}
