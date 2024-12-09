using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.ConferenceManagement
{
    public class TransferParticipantTests : ConferenceManagementControllerTestBase
    {
        [Test]
        public async Task should_move_participant_into_hearing_room_from_waiting_room()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First(x => x.UserRole == UserRole.Individual);
            participant.CurrentConsultationRoom = null;
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();
            
            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), RoomType.WaitingRoom.ToString(),
                    RoomType.HearingRoom.ToString()), Times.Once);
            VerifySupplierUsed(TestConference.Supplier, Times.Exactly(1));
        }

        [Test]
        public async Task should_move_participant_into_hearing_room_from_consultation_room()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First(x => x.UserRole == UserRole.Individual);
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };

            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();

            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), participant.CurrentConsultationRoom.Label,
                    RoomType.HearingRoom.ToString()), Times.Once);
        }

        [Test]
        public async Task should_move_room_into_hearing_room_from_waiting_room()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First(x => x.UserRole == UserRole.Individual);
            participant.CurrentConsultationRoom = null;
            var interpreterRoom = new ParticipantRoom(TestConference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), 999);
            var roomParticipant = new RoomParticipant(participant.Id)
            {
                Room = interpreterRoom,
                RoomId = interpreterRoom.Id
            };
            interpreterRoom.AddParticipant(roomParticipant);
            participant.RoomParticipants.Add(roomParticipant);
            UpdateConferenceQueryMock();
            
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();

            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, interpreterRoom.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString()), Times.Once);
        }

        [Test]
        public async Task should_move_room_into_hearing_room_from_consultation_room()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First(x => x.UserRole == UserRole.Individual);
            var interpreterRoom = new ParticipantRoom(TestConference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), 999);
            var roomParticipant = new RoomParticipant(participant.Id)
            {
                Room = interpreterRoom,
                RoomId = interpreterRoom.Id
            };
            interpreterRoom.AddParticipant(roomParticipant);
            participant.RoomParticipants.Add(roomParticipant);
            UpdateConferenceQueryMock();

            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };

            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();

            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, interpreterRoom.Id.ToString(),
                    participant.CurrentConsultationRoom.Label, RoomType.HearingRoom.ToString()), Times.Once);
        }

        [Test]
        public async Task should_move_endpoint_into_hearing_room()
        {
            var conferenceId = TestConference.Id;
            var endpoint = TestConference.Endpoints[0];
            var request = new TransferParticipantRequest
            {
                ParticipantId = endpoint.Id,
                TransferType = TransferType.Call
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();

            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), RoomType.WaitingRoom.ToString(),
                    RoomType.HearingRoom.ToString()), Times.Once);
        }
        
        [Test]
        public async Task should_dismiss_participant_from_hearing_room()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First(x => x.UserRole == UserRole.Individual);
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Dismiss
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();

            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), RoomType.HearingRoom.ToString(),
                    RoomType.WaitingRoom.ToString()), Times.Once);
        }

        [Test]
        public async Task should_dismiss_endpoint_from_hearing_room()
        {
            var conferenceId = TestConference.Id;
            var endpoint = TestConference.Endpoints[0];
            var request = new TransferParticipantRequest
            {
                ParticipantId = endpoint.Id,
                TransferType = TransferType.Dismiss
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();
            
            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), RoomType.HearingRoom.ToString(),
                    RoomType.WaitingRoom.ToString()), Times.Once);
        }

        [Test]
        public async Task should_return_NotFound_if_id_does_not_exist_for_endpoint_or_participant()
        {
            var conferenceId = TestConference.Id;
            var request = new TransferParticipantRequest
            {
                ParticipantId = Guid.NewGuid(),
                TransferType = TransferType.Dismiss
            };
            
            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<NotFoundObjectResult>();
            
            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, It.IsAny<string>(), RoomType.HearingRoom.ToString(),
                    RoomType.WaitingRoom.ToString()), Times.Never);
        }
        
        [Test]
        public void should_return_internal_server_error_when_transfer_type_is_not_handled()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First(x => x.UserRole == UserRole.Individual);
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = null
            };

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                Controller.TransferParticipantAsync(conferenceId, request));
            
            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCase(UserRole.QuickLinkObserver)]
        [TestCase(UserRole.QuickLinkParticipant)]
        public async Task Room_To_Transfer_From_Is_Set_To_Consutation_Room_For_Valid_Request(UserRole userRole)
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First();
            participant.UserRole = userRole;
            participant.CurrentConsultationRoom = new ConsultationRoom(conferenceId, "consultation room", VirtualCourtRoomType.Participant, false);
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };

            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();


            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), participant.CurrentConsultationRoom.Label,
                    RoomType.HearingRoom.ToString()), Times.Once);

        }

        [Test]
        [TestCase(UserRole.QuickLinkObserver)]
        [TestCase(UserRole.QuickLinkParticipant)]
        public async Task Room_To_Transfer_From_Is_Set_To_Waiting_Room_When_ConsultationRoom_Is_Invalid(UserRole userRole)
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First();
            participant.UserRole = userRole;
            participant.CurrentConsultationRoom = null;
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };

            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();


            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), RoomType.WaitingRoom.ToString(),
                    RoomType.HearingRoom.ToString()), Times.Once);

        }

        [Test]
        [TestCase(UserRole.QuickLinkObserver, null)]
        [TestCase(UserRole.QuickLinkParticipant, "")]
        public async Task Room_To_Transfer_From_Is_Set_To_Waiting_Room_When_ConsultationRoomLabel_Is_Invalid(UserRole userRole, string roomLabel)
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.Participants.First();
            participant.UserRole = userRole;
            participant.CurrentConsultationRoom = new ConsultationRoom(conferenceId, roomLabel, VirtualCourtRoomType.Participant, false);
            var request = new TransferParticipantRequest
            {
                ParticipantId = participant.Id,
                TransferType = TransferType.Call
            };

            var result = await Controller.TransferParticipantAsync(conferenceId, request);
            result.Should().BeOfType<AcceptedResult>();


            VideoPlatformServiceMock.Verify(
                x => x.TransferParticipantAsync(conferenceId, request.ParticipantId.ToString(), RoomType.WaitingRoom.ToString(),
                    RoomType.HearingRoom.ToString()), Times.Once);

        }
    }
}
