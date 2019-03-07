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
    public class LeaveEventHandlerTests : EventHandlerTestBase
    {
        private LeaveEventHandler _eventHandler;

        [Test]
        public async Task should_send_available_message_to_participants_and_service_bus_when_participant_joins()
        {
            _eventHandler = new LeaveEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var participantCount = conference.GetParticipants().Count;

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Leave,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id.ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(_eventHandler.SourceParticipant.Username,
                    ParticipantEventStatus.Unavailable), Times.Exactly(participantCount));

            ServiceBusQueueClient.Count.Should().Be(1);
            var participantMessage = (ParticipantEventMessage) ServiceBusQueueClient.ReadMessageFromQueue();
            participantMessage.Should().BeOfType<ParticipantEventMessage>();
            participantMessage.ParticipantEventStatus.Should().Be(ParticipantEventStatus.Unavailable);
        }
    }
}