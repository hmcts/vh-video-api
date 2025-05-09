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
using VideoApi.Services.Clients.Models;

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
                .Where(x => x.CanAutoTransferToHearingRoom() && !x.IsHost()).Select(x => x.Id);
            var hostsForScreening = new List<Guid> { participantIds.First() };
            var hostsForScreeningAsStrings = hostsForScreening.Select(x => x.ToString()).ToList();

            participantIds = participantIds.Append(hostId).ToList();
            var participantIdsAsStrings = participantIds.Select(x => x.ToString()).ToList();
            var hostIds = new List<Guid>(participantIds);
            var hostIdsAsStrings = hostIds.Select(x => x.ToString()).ToList();
            var muteGuests = true;
            var request = new Contract.Requests.StartHearingRequest
            {
                Layout = layout,
                TriggeredByHostId = hostId,
                MuteGuests = true,
                Hosts = hostIds,
                HostsForScreening = hostsForScreening
            };
            TestConference.SetProtectedProperty(nameof(TestConference.Supplier), Supplier.Vodafone);
            Mocker.Mock<IQueryHandler>()
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(
                    It.Is<GetConferenceByIdQuery>(q => q.ConferenceId == TestConference.Id)))
                .ReturnsAsync(TestConference);
            
            var result = await Controller.StartVideoHearingAsync(conferenceId, request);

            var typedResult = (AcceptedResult) result;
            typedResult.Should().NotBeNull();
            typedResult.StatusCode.Should().Be((int) HttpStatusCode.Accepted);
            VideoPlatformServiceMock.Verify(
                x => x.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(), participantIdsAsStrings,
                    hostIdsAsStrings, Layout.OnePlusSeven, muteGuests, hostsForScreeningAsStrings), Times.Once);
            VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
        }
        
        [Test]
        public async Task should_return_accepted_when_start_hearing_with_muted_guests_has_been_requested_when_supplier_is_vodafone()
        {
            var conferenceId = TestConference.Id;
            var layout = HearingLayout.OnePlus7;
            var hostId = TestConference.Participants.Single(x => x.UserRole == VideoApi.Domain.Enums.UserRole.Judge).Id;
            var participantIds = TestConference.Participants
                .Where(x => x.CanAutoTransferToHearingRoom() && !x.IsHost()).Select(x => x.Id);
            var hostsForScreening = new List<Guid> { participantIds.First() };
            var hostsForScreeningAsStrings = hostsForScreening.Select(x => x.ToString()).ToList();

            participantIds = participantIds.Append(hostId).ToList();
            var participantIdsAsStrings = participantIds.Select(x => x.ToString()).ToList();
            var hostIds = new List<Guid>(participantIds);
            var hostIdsAsStrings = hostIds.Select(x => x.ToString()).ToList();
            var request = new Contract.Requests.StartHearingRequest
            {
                Layout = layout,
                TriggeredByHostId = hostId,
                MuteGuests = true,
                Hosts = hostIds,
                HostsForScreening = hostsForScreening
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
                x => x.StartHearingAsync(conferenceId, request.TriggeredByHostId.ToString(), participantIdsAsStrings, hostIdsAsStrings,
                    Layout.OnePlusSeven, true, hostsForScreeningAsStrings), Times.Once);
        }

        [Test]
        public async Task should_contain_correct_participants_when_start_hearing_has_been_requested()
        {
            var conferenceId = TestConference.Id;
            AddTelephoneParticipantToTestConference();
            AddWitnessToTestConference();
            AddExpertToTestConference();
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
                    It.IsAny<IEnumerable<string>>(), Layout.OnePlusSeven, true, It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
