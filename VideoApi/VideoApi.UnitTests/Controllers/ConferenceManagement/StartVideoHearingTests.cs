using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Clients;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class StartVideoHearingTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_return_accepted_when_start_hearing_has_been_requested()
        {
            var conferenceId = TestConference.Id;
            var layout = HearingLayout.OnePlus7;
            var hostId = TestConference.Participants.Single(x => x.UserRole == VideoApi.Domain.Enums.UserRole.Judge).Id;
            var participantIds = TestConference.Participants
                .Where(x => x.CanAutoTransferToHearingRoom() && !x.IsHost()).Select(x => x.Id.ToString());
            participantIds = participantIds.Append(hostId.ToString()).ToList();
            var hostIds = new List<string>(participantIds);
            var muteGuests = true;
            var request = new Contract.Requests.StartHearingRequest
            {
                Layout = layout,
                TriggeredByHostId = hostId,
                MuteGuests = true
            };
            TestConference.SetProtectedProperty(nameof(TestConference.Supplier), Supplier.Kinly);
            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId, request);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(
                x => x.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(), participantIds,
                    hostIds, Layout.ONE_PLUS_SEVEN, muteGuests), Times.Once);
            VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
        }
        
        [Test]
        public async Task should_return_accepted_when_start_hearing_with_muted_guests_has_been_requested_when_supplier_is_vodafone()
        {
            var conferenceId = TestConference.Id;
            var layout = HearingLayout.OnePlus7;
            var hostId = TestConference.Participants.Single(x => x.UserRole == VideoApi.Domain.Enums.UserRole.Judge).Id;
            var participantIds = TestConference.Participants
                .Where(x => x.CanAutoTransferToHearingRoom() && !x.IsHost()).Select(x => x.Id.ToString());
            participantIds = participantIds.Append(hostId.ToString()).ToList();
            participantIds = participantIds.Append(hostId.ToString()).ToList();
            var hostIds = new List<string>(participantIds);
            var request = new Contract.Requests.StartHearingRequest
            {
                Layout = layout,
                TriggeredByHostId = hostId,
                MuteGuests = true
            };
            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);
            TestConference.SetProtectedProperty(nameof(TestConference.Supplier), Supplier.Vodafone);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId, request);
            
            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(
                x => x.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(), participantIds, hostIds,
                    Layout.ONE_PLUS_SEVEN, true), Times.Once);
        }

        [Test] public async Task should_return_supplier_status_code_on_error()
        {
            var conferenceId = TestConference.Id;
            var message = "Auto Test Error";
            var response = "You're not allowed to start this hearing";
            var statusCode = (int) HttpStatusCode.Unauthorized;
            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);
            var exception = new SupplierApiException(message, statusCode, response, null, null);
            VideoPlatformServiceMock.Setup(x => x.StartHearingAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<Layout>(), It.IsAny<bool>()))
                .ThrowsAsync(exception);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId, new Contract.Requests.StartHearingRequest());
            var typedResult = (ObjectResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().Be(response);
        }

        [Test]
        public async Task should_return_bad_request_when_user_a_supplier_api_error_is_thrown_with_400()
        {
            var conferenceId = TestConference.Id;
            var message = "Auto Test Error";
            var response = "No participants to transfer";
            var statusCode = (int) HttpStatusCode.BadRequest;
            var exception = new SupplierApiException(message, statusCode, response, null, null);
            
            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);

            VideoPlatformServiceMock.Setup(x => x.StartHearingAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>>(), It.IsAny<Layout>(), It.IsAny<bool>()))
                .ThrowsAsync(exception);

            var result =
                await Controller.StartVideoHearingAsync(conferenceId, new Contract.Requests.StartHearingRequest());
            var typedResult = result.Should().BeAssignableTo<BadRequestObjectResult>().Subject;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be(statusCode);
            typedResult.Value.Should().BeAssignableTo<string>().Which.Should()
                .Contain("Invalid list of participants provided for");
        }
        
        [Test]
        public async Task should_contain_correct_participants_when_start_hearing_has_been_requested()
        {
            var conferenceId = TestConference.Id;
            AddTelephoneParticipantToTestConference();
            AddWitnessToTestConference();
            AddQuicklinkToTestConference();

            var layout = HearingLayout.OnePlus7;
            var hostId = TestConference.Participants.Single(x => x.UserRole == VideoApi.Domain.Enums.UserRole.Judge).Id;
            var participantIds = TestConference.Participants
                .Where(x => x.CanAutoTransferToHearingRoom() &&
                            !x.IsHost()).Select(x => x.Id.ToString()).ToList();
            
            var endpoints = TestConference.Endpoints
                .Where(x => x.State is EndpointState.Connected or EndpointState.InConsultation)
                .Select(x => x.Id.ToString());

            participantIds.AddRange(endpoints);
            participantIds.Add(TestConference.GetTelephoneParticipants()[0].Id.ToString());
            participantIds.Add(hostId.ToString());
            
            var request = new Contract.Requests.StartHearingRequest
            {
                Layout = layout,
                TriggeredByHostId = hostId,
                MuteGuests = true
            };

            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);

            var result = await Controller.StartVideoHearingAsync(conferenceId, request);

            var typedResult = (AcceptedResult)result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(
                x => x.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(), participantIds,
                    It.IsAny<IEnumerable<string>>(), Layout.ONE_PLUS_SEVEN, true), Times.Once);
        }
    }
}
