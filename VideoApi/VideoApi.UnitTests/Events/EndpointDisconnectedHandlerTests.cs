using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
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
            var participantForEvent = conference.GetEndpoints().First();

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointDisconnected,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateEndpointStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                EndpointState.Disconnected, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id && command.EndpointId == participantForEvent.Id &&
                    command.Status == EndpointState.Disconnected && command.Room == null)), Times.Once);
        }
    }
}
