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
    public class JoinedEventHandlerTests : EventHandlerTestBase
    {
        private JoinedEventHandler _eventHandler;

        [Test]
        public async Task should_send_available_message_to_participants_and_service_bus_when_participant_joins()
        {
            _eventHandler = new JoinedEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var participantCount = conference.GetParticipants().Count;

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Joined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id.ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(_eventHandler.SourceParticipant.Username,
                    ParticipantEventStatus.Available), Times.Exactly(participantCount));

            ServiceBusQueueClient.Count.Should().Be(1);
            var participantMessage = (ParticipantEventMessage) ServiceBusQueueClient.ReadMessageFromQueue();
            participantMessage.Should().BeOfType<ParticipantEventMessage>();
            participantMessage.ParticipantEventStatus.Should().Be(ParticipantEventStatus.Available);
        }

        [Test]
        public async Task
            should_send_in_hearing_message_to_participants_and_live_message_to_service_bus_when_judge_joins()
        {
            _eventHandler = new JoinedEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var participantCount = conference.GetParticipants().Count;

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Joined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id.ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            // Verify event hub client
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(_eventHandler.SourceParticipant.Username,
                    ParticipantEventStatus.InHearing), Times.Exactly(participantCount));

            EventHubClientMock.Verify(
                x => x.HearingStatusMessage(conference.HearingRefId, HearingEventStatus.Live),
                Times.Exactly(participantCount));

            // Verify service bus
            ServiceBusQueueClient.Count.Should().Be(2);

            var participantEventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            participantEventMessage.Should().BeOfType<ParticipantEventMessage>();
            ((ParticipantEventMessage) participantEventMessage).ParticipantEventStatus.Should()
                .Be(ParticipantEventStatus.InHearing);
            participantEventMessage.MessageType.Should().Be(MessageType.Participant);

            var hearingEventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            hearingEventMessage.Should().BeOfType<HearingEventMessage>();
            ((HearingEventMessage) hearingEventMessage).HearingEventStatus.Should().Be(HearingEventStatus.Live);
            hearingEventMessage.MessageType.Should().Be(MessageType.Hearing);
        }
    }
}