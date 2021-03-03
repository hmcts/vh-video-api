using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Events
{
    public class TransferEventHandlerTests : EventHandlerTestBase<TransferEventHandler>
    {
        [TestCase(RoomType.WaitingRoom, RoomType.HearingRoom, ParticipantState.InHearing)]
        [TestCase(RoomType.HearingRoom, RoomType.WaitingRoom, ParticipantState.Available)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom, ParticipantState.InConsultation)]
        [TestCase(RoomType.ConsultationRoom, RoomType.WaitingRoom, ParticipantState.Available)]
        [TestCase(RoomType.ConsultationRoom, RoomType.HearingRoom, ParticipantState.InHearing)]
        public async Task Should_send_participant__status_messages_to_clients_and_asb_when_transfer_occurs(
            RoomType from, RoomType to, ParticipantState status)
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = from,
                TransferTo = to,
                TransferredFromRoomLabel = from.ToString(),
                TransferredToRoomLabel = to.ToString(),
                
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == status &&
                    command.Room == to)), Times.Once);
        }

        [Test]
        public async Task Should_map_to_in_consultation_status_when_transfer_to_label_contains_consultation()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = RoomType.WaitingRoom,
                TransferTo = null,
                TransferredFromRoomLabel = RoomType.WaitingRoom.ToString(),
                TransferredToRoomLabel = "JudgeConsultationRoom3",
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.InConsultation && 
                    command.Room == null && 
                    command.RoomLabel == callbackEvent.TransferredToRoomLabel)), Times.Once);
        }
        
        [Test]
        public async Task Should_map_to_in_hearing_status_when_transfer_from_new_consultation_room_to_hearing_room()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = RoomType.ConsultationRoom,
                TransferredFromRoomLabel = "JudgeConsultationRoom3",
                TransferTo = RoomType.HearingRoom,
                TransferredToRoomLabel = RoomType.HearingRoom.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.InHearing && 
                    command.Room == RoomType.HearingRoom &&
                    command.RoomLabel == RoomType.HearingRoom.ToString())), Times.Once);
        }
        
        [Test]
        public async Task Should_map_to_available_status_when_transfer_to_waiting_room_from_judge_consultation_room()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            QueryHandlerMock.Setup(x => x.Handle<GetRoomByIdQuery, Room>(It.IsAny<GetRoomByIdQuery>())).ReturnsAsync(new Room(conference.Id, "JudgeConsultationRoom3", VirtualCourtRoomType.JudgeJOH, false));

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = null,
                TransferTo = RoomType.WaitingRoom,
                TransferredFromRoomLabel = "JudgeConsultationRoom3",
                TransferredToRoomLabel = RoomType.WaitingRoom.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available && 
                    command.Room == RoomType.WaitingRoom && 
                    command.RoomLabel == RoomType.WaitingRoom.ToString())), Times.Once);
        }

        [Test]
        public async Task Should_transfer_endpoints_out_of_room_when_last_participant_leaves()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var room = new Room(conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(new RoomEndpoint(Guid.NewGuid()));
            room.AddEndpoint(new RoomEndpoint(Guid.NewGuid()));
            QueryHandlerMock.Setup(x => x.Handle<GetRoomByIdQuery, Room>(It.IsAny<GetRoomByIdQuery>())).ReturnsAsync(room);

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = null,
                TransferTo = RoomType.WaitingRoom,
                TransferredFromRoomLabel = "ConsultationRoom2",
                TransferredToRoomLabel = RoomType.WaitingRoom.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available &&
                    command.Room == RoomType.WaitingRoom &&
                    command.RoomLabel == RoomType.WaitingRoom.ToString())), Times.Once);

            foreach (var roomEndpoint in room.RoomEndpoints)
            {
                _mocker.Mock<IConsultationService>().Verify(x => x.EndpointTransferToRoomAsync(conference.Id, roomEndpoint.EndpointId, RoomType.WaitingRoom.ToString()), Times.Once);
            }
        }

        [Test]
        public async Task Should_transfer_endpoints_out_of_room_when_defense_advocate_participant_leaves_and_room_not_empty()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Representative && x.Username == "DA1@test.com");
            var room = new Room(conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);
            room.AddParticipant(new RoomParticipant(Guid.NewGuid()));
            room.AddParticipant(new RoomParticipant(Guid.NewGuid()));
            foreach (var endpoint in conference.GetEndpoints())
            {
                room.AddEndpoint(new RoomEndpoint(endpoint.Id));
            }

            QueryHandlerMock.Setup(x => x.Handle<GetRoomByIdQuery, Room>(It.IsAny<GetRoomByIdQuery>())).ReturnsAsync(room);

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = null,
                TransferTo = RoomType.WaitingRoom,
                TransferredFromRoomLabel = "ConsultationRoom2",
                TransferredToRoomLabel = RoomType.WaitingRoom.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available &&
                    command.Room == RoomType.WaitingRoom &&
                    command.RoomLabel == RoomType.WaitingRoom.ToString())), Times.Once);

            _mocker.Mock<IConsultationService>().Verify(x => x.EndpointTransferToRoomAsync(conference.Id, It.IsAny<Guid>(), RoomType.WaitingRoom.ToString()), Times.Once);
        }

        [Test]
        public async Task Should_not_transfer_endpoints_out_of_room_when_last_participant_leaves_if_transferring_to_hearing()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var room = new Room(conference.Id, "ConsultationRoom2", VirtualCourtRoomType.Participant, false);
            room.AddEndpoint(new RoomEndpoint(Guid.NewGuid()));
            room.AddEndpoint(new RoomEndpoint(Guid.NewGuid()));
            QueryHandlerMock.Setup(x => x.Handle<GetRoomByIdQuery, Room>(It.IsAny<GetRoomByIdQuery>())).ReturnsAsync(room);

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TransferFrom = null,
                TransferTo = RoomType.HearingRoom,
                TransferredFromRoomLabel = "ConsultationRoom2",
                TransferredToRoomLabel = RoomType.HearingRoom.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };
            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.InHearing &&
                    command.Room == RoomType.HearingRoom &&
                    command.RoomLabel == RoomType.HearingRoom.ToString())), Times.Once);

            _mocker.Mock<IConsultationService>().Verify(x => x.EndpointTransferToRoomAsync(conference.Id, It.IsAny<Guid>(), RoomType.WaitingRoom.ToString()), Times.Never);
        }
    }
}
