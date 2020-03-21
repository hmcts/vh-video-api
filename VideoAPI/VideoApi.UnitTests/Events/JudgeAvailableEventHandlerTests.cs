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
    public class JudgeAvailableEventHandlerTests : EventHandlerTestBase
    {
        private JudgeAvailableEventHandler _eventHandler;

        [Test]
        public async Task Should_send_available_participant_messages_when_judge_available()
        {
            _eventHandler = new JudgeAvailableEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient);

            var conference = TestConference;
            
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.JudgeAvailable,
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
                    command.ParticipantState == ParticipantState.Available)), Times.Once);
        }
    }
}