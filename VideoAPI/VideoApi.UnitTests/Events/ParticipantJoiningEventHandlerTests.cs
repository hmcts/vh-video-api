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
    public class ParticipantJoiningEventHandlerTests : EventHandlerTestBase<ParticipantJoiningEventHandler>
    {
        [Test]
        public async Task Should_send_joining_message_to_participants_and_service_bus_when_a_participant_is_joining()
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
            var updateStatusCommand = new UpdateParticipantStatusCommand(conference.Id, participantForEvent.Id,
                ParticipantState.Joining);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Joining)), Times.Once);
        }
    }
}
