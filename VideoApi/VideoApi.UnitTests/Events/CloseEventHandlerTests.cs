using System;
using System.Threading.Tasks;
using Moq;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class CloseEventHandlerTests : EventHandlerTestBase<CloseEventHandler>
    {
        [Test]
        public async Task Should_send_messages_to_participants_and_service_bus_on_close()
        {
            var conference = TestConference;
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Close,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateConferenceStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ConferenceState == ConferenceState.Closed)), Times.Once);
        }
    }
}
