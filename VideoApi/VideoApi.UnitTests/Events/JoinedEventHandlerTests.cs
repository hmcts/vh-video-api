using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class JoinedEventHandlerTests : EventHandlerTestBase<JoinedEventHandler>
    {
        [Test]
        public async Task Should_send_available_message_to_participants_and_service_bus_when_participant_joins()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Joined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateParticipantStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                ParticipantState.Available, RoomType.WaitingRoom, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available &&
                    command.Room == RoomType.WaitingRoom)), Times.Once);
        }
    }
}
