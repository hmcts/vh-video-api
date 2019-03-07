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
    public class HelpEventHandlerTests : EventHandlerTestBase
    {
        private HelpEventHandler _eventHandler;

        [Test]
        public async Task should_send_messages_to_participants_and_service_bus_on_disconnect()
        {
            _eventHandler = new HelpEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Help,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                ConferenceId = conference.Id.ToString()
            };

            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.HelpMessage(conference.HearingRefId, participantForEvent.DisplayName), Times.Once);
        }
    }
}