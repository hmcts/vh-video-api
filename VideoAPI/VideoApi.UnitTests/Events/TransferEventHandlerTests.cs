using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class TransferEventHandlerTests : EventHandlerTestBase<TransferEventHandler>
    {
        [TestCase(RoomType.WaitingRoom, RoomType.HearingRoom, ParticipantState.InHearing)]
        [TestCase(RoomType.HearingRoom, RoomType.WaitingRoom, ParticipantState.Available)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom1, ParticipantState.InConsultation)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom2, ParticipantState.InConsultation)]
        [TestCase(RoomType.ConsultationRoom1, RoomType.WaitingRoom, ParticipantState.Available)]
        [TestCase(RoomType.ConsultationRoom2, RoomType.WaitingRoom, ParticipantState.Available)]
        [TestCase(RoomType.ConsultationRoom1, RoomType.HearingRoom, ParticipantState.InHearing)]
        [TestCase(RoomType.ConsultationRoom2, RoomType.HearingRoom, ParticipantState.InHearing)]
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
        public async Task Should_map_to_to_in_consultation_status_when_transfer_to_label_contains_consultation()
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
        public async Task Should_map_to_to_available_status_when_transfer_to_waiting_room_from_judge_consultation_room()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);

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
        public void Should_throw_exception_when_transfer_cannot_be_mapped_to_participant_status()
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
                TransferTo = RoomType.WaitingRoom,
                TransferredFromRoomLabel = RoomType.WaitingRoom.ToString(),
                TransferredToRoomLabel = RoomType.WaitingRoom.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };

            Assert.ThrowsAsync<RoomTransferException>(() =>
                _sut.HandleAsync(callbackEvent));

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == It.IsAny<ParticipantState>())), Times.Never);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateConferenceStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ConferenceState == ConferenceState.InSession)), Times.Never);
        }
    }
}
