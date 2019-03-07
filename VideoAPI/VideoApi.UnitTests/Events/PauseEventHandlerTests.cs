using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class PauseEventHandlerTests : EventHandlerTestBase
    {
        private PauseEventHandler _pauseEventHandler;


        [Test]
        public async Task should_send_paused_hearing_messages_to_clients_and_service_bus_when_hearing_is_paused()
        {
            _pauseEventHandler = new PauseEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);
            var conference = new ConferenceBuilder().WithParticipants(2).Build();
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(conference);

            foreach (var participant in conference.GetParticipants())
            {
                EventHubContextMock.Setup(x => x.Clients.Group(participant.Username.ToString()))
                    .Returns(EventHubClientMock.Object);
            }

            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Pause,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                ConferenceId = conference.Id.ToString()
            };

            await _pauseEventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(x => x.HearingStatusMessage(conference.HearingRefId, "Paused"),
                Times.Exactly(conference.GetParticipants().Count));

            // Verify messages sent to ASB queue
            ServiceBusQueueClient.Count.Should().Be(1);

            var hearingEventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            hearingEventMessage.Should().BeOfType<HearingEventMessage>();
            ((HearingEventMessage) hearingEventMessage).HearingStatus.Should().Be("Paused");
        }
    }
}