using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.Services.Clients;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class StartVideoHearingTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_return_accepted_when_start_hearing_has_been_requested()
        {
            var conferenceId = Guid.NewGuid();
            var layout = HearingLayout.OnePlus7;
            var participants = new[] {"participant-one", "participant-two"};
            var muteGuests = true;
            var request = new Contract.Requests.StartHearingRequest
            {
                Layout = layout,
                ParticipantsToForceTransfer = participants,
                MuteGuests = true
            };
            var result = await Controller.StartVideoHearingAsync(conferenceId, request);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(x => x.StartHearingAsync(conferenceId, participants, Layout.ONE_PLUS_SEVEN, muteGuests), Times.Once);
        }

        [Test] public async Task should_return_kinly_status_code_on_error()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "You're not allowed to start this hearing";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            var exception =
                new SupplierApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.StartHearingAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<string>>(), It.IsAny<Layout>(), It.IsAny<bool>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId, new Contract.Requests.StartHearingRequest());
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }

        [Test]
        public async Task should_return_bad_request_when_user_a_kinly_api_error_is_thrown_with_400()
        {
            var conferenceId = Guid.NewGuid();
            var message = "Auto Test Error";
            var response = "No participants to transfer";
            var statusCode = (int) HttpStatusCode.BadRequest;
            var exception =
                new SupplierApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.StartHearingAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<string>>(),
                    It.IsAny<Layout>(), It.IsAny<bool>()))
                .ThrowsAsync(exception);

            var result =
                await Controller.StartVideoHearingAsync(conferenceId, new Contract.Requests.StartHearingRequest());
            var typedResult = result.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().BeAssignableTo<string>().Which.Should()
                .Contain("Invalid list of participants provided for");
        }
    }
}
