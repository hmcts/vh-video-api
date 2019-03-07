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
    public class TransferEventHandlerTests : EventHandlerTestBase
    {
        private TransferEventHandler _eventHandler;

        [TestCase(RoomType.WaitingRoom, RoomType.HearingRoom, ParticipantEventStatus.InHearing)]
        [TestCase(RoomType.HearingRoom, RoomType.WaitingRoom, ParticipantEventStatus.Available)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom1, ParticipantEventStatus.InConsultation)]
        [TestCase(RoomType.WaitingRoom, RoomType.ConsultationRoom2, ParticipantEventStatus.InConsultation)]
        [TestCase(RoomType.ConsultationRoom1, RoomType.WaitingRoom, ParticipantEventStatus.Available)]
        [TestCase(RoomType.ConsultationRoom2, RoomType.WaitingRoom, ParticipantEventStatus.Available)]
        public async Task should_send_participant__status_messages_to_clients_and_asb_when_transfer_occurs(
            RoomType from, RoomType to, ParticipantEventStatus status)
        {
            _eventHandler = new TransferEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var participantCount = conference.GetParticipants().Count;


            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Transfer,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id.ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                TransferFrom = from,
                TransferTo = to
            };
            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(_eventHandler.SourceParticipant.Username,
                    status), Times.Exactly(participantCount));

            // Verify messages sent to ASB queue
            ServiceBusQueueClient.Count.Should().Be(1);

            var participantEventMessage = ServiceBusQueueClient.ReadMessageFromQueue();
            participantEventMessage.Should().BeOfType<ParticipantEventMessage>();
            ((ParticipantEventMessage) participantEventMessage).ParticipantEventStatus.Should().Be(status);
        }
    }
}