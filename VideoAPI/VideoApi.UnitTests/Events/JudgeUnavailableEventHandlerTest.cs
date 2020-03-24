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
    public class JudgeUnavailableEventHandlerTest : EventHandlerTestBase
    {
        private JudgeUnavailableEventHandler _eventHandler;

        [Test]
        public async Task Should_send_unavailable_participant_messages_when_judge_unavailable()
        {
            _eventHandler = new JudgeUnavailableEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object);

            var conference = TestConference;

            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.JudgeUnavailable,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.NotSignedIn)), Times.Once);
        }
    }
}
