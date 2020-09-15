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
    public class EndpointTransferredEventHandlerTests : EventHandlerTestBase
    {
        private EndpointTransferredEventHandler _eventHandler;
        
        [TestCase(RoomType.WaitingRoom, RoomType.HearingRoom, EndpointState.Connected)]
        [TestCase(RoomType.HearingRoom, RoomType.WaitingRoom, EndpointState.Connected)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom1, EndpointState.InConsultation)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom2, EndpointState.InConsultation)]
        [TestCase(RoomType.ConsultationRoom1, RoomType.WaitingRoom, EndpointState.Connected)]
        [TestCase(RoomType.ConsultationRoom2, RoomType.WaitingRoom, EndpointState.Connected)]
        [TestCase(RoomType.ConsultationRoom1, RoomType.HearingRoom, EndpointState.Connected)]
        [TestCase(RoomType.ConsultationRoom2, RoomType.HearingRoom, EndpointState.Connected)]
        public async Task Should_send_participant__status_messages_to_clients_and_asb_when_transfer_occurs(RoomType from, RoomType to, EndpointState status)
        {
            _eventHandler = new EndpointTransferredEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object, RoomReservationService);

            var conference = TestConference;
            var endpointForEvent = conference.GetEndpoints().First();
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointTransfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = endpointForEvent.Id,
                TransferFrom = from,
                TransferTo = to,
                TimeStampUtc = DateTime.UtcNow
            };
            await _eventHandler.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.EndpointId == endpointForEvent.Id &&
                    command.Status == status &&
                    command.Room == to)), Times.Once);
        }
        
        [Test]
        public void Should_throw_exception_when_transfer_cannot_be_mapped_to_endpoint_status()
        {
            _eventHandler = new EndpointTransferredEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object, RoomReservationService);

            var conference = TestConference;
            var endpointForEvent = conference.GetEndpoints().First();

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = endpointForEvent.Id,
                TransferFrom = RoomType.WaitingRoom,
                TransferTo = RoomType.WaitingRoom,
                TimeStampUtc = DateTime.UtcNow
            };

            Assert.ThrowsAsync<RoomTransferException>(() =>
                _eventHandler.HandleAsync(callbackEvent));

            CommandHandlerMock.Verify(
                x => x.Handle(It.IsAny<UpdateEndpointStatusAndRoomCommand>()), Times.Never);
        }
    }
    public class TransferEventHandlerTests : EventHandlerTestBase
    {
        private TransferEventHandler _eventHandler;

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
            _eventHandler = new TransferEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object, RoomReservationService);

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
                TimeStampUtc = DateTime.UtcNow
            };
            await _eventHandler.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == status &&
                    command.Room == to)), Times.Once);
        }

        [Test]
        public void Should_throw_exception_when_transfer_cannot_be_mapped_to_participant_status()
        {
            _eventHandler = new TransferEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object, RoomReservationService);

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
                TimeStampUtc = DateTime.UtcNow
            };

            Assert.ThrowsAsync<RoomTransferException>(() =>
                _eventHandler.HandleAsync(callbackEvent));

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
