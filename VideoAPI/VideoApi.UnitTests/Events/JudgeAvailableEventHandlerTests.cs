using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Events.Models.Enums;

namespace VideoApi.UnitTests.Events
{
    public class JudgeAvailableEventHandlerTests : EventHandlerTestBase
    {
        private JudgeAvailableEventHandler _eventHandler;

        [Test]
        public async Task should_send_available_participant_messages_when_judge_available()
        {
            _eventHandler = new JudgeAvailableEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantCount = conference.GetParticipants().Count;
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
                x => x.ParticipantStatusMessage(participantForEvent.Username, ParticipantEventStatus.Available),
                Times.Exactly(participantCount));

            ServiceBusQueueClient.Count.Should().Be(1);
            var participantMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            participantMessage.Should().BeOfType<ParticipantEventMessage>();
            ((ParticipantEventMessage) participantMessage).ParticipantEventStatus.Should()
                .Be(ParticipantEventStatus.Available);
        }
    }
}