using System;
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
    public class PauseEventHandlerTests : EventHandlerTestBase
    {
        private PauseEventHandler _eventHandler;

        [Test]
        public async Task should_send_messages_to_participants_and_service_bus_on_pause()
        {
            _eventHandler = new PauseEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Pause,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(x => x.HearingStatusMessage(conference.HearingRefId, HearingEventStatus.Paused),
                Times.Exactly(conference.GetParticipants().Count));

            // Verify messages sent to ASB queue
            ServiceBusQueueClient.Count.Should().Be(1);

            var eventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            eventMessage.Should().BeOfType<HearingEventMessage>();
            ((HearingEventMessage) eventMessage).HearingEventStatus.Should().Be(HearingEventStatus.Paused);
        }
    }
}