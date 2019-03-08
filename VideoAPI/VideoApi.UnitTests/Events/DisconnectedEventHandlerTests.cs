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
    public class DisconnectedEventHandlerTests : EventHandlerTestBase
    {
        private DisconnectedEventHandler _eventHandler;

        [Test]
        public async Task should_send_disconnect_messages_to_participants_and_service_bus_on_participant_disconnect()
        {
            _eventHandler = new DisconnectedEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Disconnected,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                Reason = "Unexpected drop",
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(participantForEvent.Username, ParticipantEventStatus.Disconnected),
                Times.Exactly(conference.GetParticipants().Count));

            // Verify messages sent to ASB queue
            ServiceBusQueueClient.Count.Should().Be(1);

            var eventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            eventMessage.Should().BeOfType<ParticipantEventMessage>();
            ((ParticipantEventMessage) eventMessage).ParticipantEventStatus.Should().Be(ParticipantEventStatus.Disconnected);
        }

        [Test]
        public async Task
            should_send_disconnect_and_suspend_messages_to_participants_and_service_bus_on_judge_disconnect()
        {
            _eventHandler = new DisconnectedEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantCount = conference.GetParticipants().Count;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Disconnected,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);
            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(_eventHandler.SourceParticipant.Username,
                    ParticipantEventStatus.Disconnected),
                Times.Exactly(participantCount));

            EventHubClientMock.Verify(
                x => x.HearingStatusMessage(conference.HearingRefId, HearingEventStatus.Suspended),
                Times.Exactly(participantCount));

            // Verify messages sent to ASB queue
            ServiceBusQueueClient.Count.Should().Be(2);

            var participantEventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            participantEventMessage.Should().BeOfType<ParticipantEventMessage>();
            ((ParticipantEventMessage) participantEventMessage).ParticipantEventStatus.Should()
                .Be(ParticipantEventStatus.Disconnected);

            var hearingEventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            hearingEventMessage.Should().BeOfType<HearingEventMessage>();
            ((HearingEventMessage) hearingEventMessage).HearingEventStatus.Should().Be(HearingEventStatus.Suspended);
        }
    }
}