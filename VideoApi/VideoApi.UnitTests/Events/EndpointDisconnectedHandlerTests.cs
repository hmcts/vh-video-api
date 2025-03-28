using System;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class EndpointDisconnectedHandlerTests : EventHandlerTestBase<EndpointDisconnectedEventHandler>
    {        
        [Test]
        public async Task Should_update_endpoint_status_to_disconnected()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetEndpoints()[0];

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointDisconnected,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateEndpointStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                EndpointState.Disconnected, null, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id && command.EndpointId == participantForEvent.Id &&
                    command.Status == EndpointState.Disconnected && command.Room == null)), Times.Once);
        }

        [Test]
        public async Task should_still_update_endpoint_status_if_endpoint_has_been_updated_since_callback_received()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetEndpoints()[0];
            participantForEvent.UpdateStatus(EndpointState.Connected);
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointDisconnected,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow.AddSeconds(-1)
            };
            
            await _sut.HandleAsync(callbackEvent);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id && command.EndpointId == participantForEvent.Id &&
                    command.Status == EndpointState.Disconnected && command.Room == null)), Times.Once);
        }
    }
}
