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
    public class EndpointJoinedHandlerTests : EventHandlerTestBase
    {
        private EndpointJoinedEventHandler _eventHandler;
        
        [Test]
        public async Task Should_send_available_message_to_participants_and_service_bus_when_participant_joins()
        {
            _eventHandler = new EndpointJoinedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetEndpoints().First();

            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.EndpointJoined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateStatusCommand = new UpdateEndpointStatusCommand(conference.Id, participantForEvent.Id,
                EndpointState.Connected);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            await _eventHandler.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateEndpointStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.EndpointId == participantForEvent.Id &&
                    command.Status == EndpointState.Connected)), Times.Once);
        }
    }
}
