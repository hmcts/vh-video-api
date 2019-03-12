using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class CloseEventHandlerTests : EventHandlerTestBase
    {
        private CloseEventHandler _eventHandler;

        [Test]
        public async Task should_send_messages_to_participants_and_service_bus_on_close()
        {
            _eventHandler = new CloseEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient, EventHubContextMock.Object);

            var conference = TestConference;
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Close,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(x => x.ConferenceStatusMessage(conference.HearingRefId, ConferenceState.Closed),
                Times.Exactly(conference.GetParticipants().Count));

            // Verify messages sent to ASB queue
            ServiceBusQueueClient.Count.Should().Be(1);

            var eventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            eventMessage.Should().BeOfType<HearingEventMessage>();
            ((HearingEventMessage) eventMessage).ConferenceStatus.Should().Be(ConferenceState.Closed);
        }
    }
}