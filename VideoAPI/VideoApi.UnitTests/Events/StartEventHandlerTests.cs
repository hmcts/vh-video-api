using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class StartEventHandlerTests : EventHandlerTestBase
    {
        private StartEventHandler _eventHandler;
        
        [Test]
        public async Task Should_send_messages_to_participants_and_service_bus_on_start()
        {
            _eventHandler = new StartEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object);

            var conference = TestConference;
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Start,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _eventHandler.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateConferenceStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ConferenceState == ConferenceState.InSession)), Times.Once);
        }
    }
}