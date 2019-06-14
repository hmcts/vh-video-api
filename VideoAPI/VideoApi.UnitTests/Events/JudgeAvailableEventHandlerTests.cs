using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
        public async Task should_send_available_participant_messages_when_judge_available()
        {
            _eventHandler = new JudgeAvailableEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient, EventHubContextMock.Object);

            var conference = TestConference;
            var participantCount = conference.GetParticipants().Count + 1; // plus one for admin
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

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(participantForEvent.Username, ParticipantState.Available),
                Times.Exactly(participantCount));

            ServiceBusQueueClient.Count.Should().Be(1);
            var participantMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            participantMessage.Should().BeOfType<ParticipantEventMessage>();
            ((ParticipantEventMessage) participantMessage).ParticipantState.Should()
                .Be(ParticipantState.Available);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available)), Times.Once);
        }
    }
}