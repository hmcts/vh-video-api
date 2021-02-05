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
    public class SuspendEventHandlerTests : EventHandlerTestBase<SuspendEventHandler>
    {
        [Test]
        public async Task Should_send_messages_to_participants_on_suspended()
        {
            var conference = TestConference;
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Pause,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow,
                ParticipantId = conference.Participants.First().Id
            };

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateConferenceStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ConferenceState == ConferenceState.Suspended)), Times.Once);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.Body == "Hearing suspended")), Times.Once);
        }

        [Test]
        public async Task Should_send_messages_to_participants_on_suspended_for_technical_assistance()
        {
            var conference = TestConference;
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Pause,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow,
            };

            await _sut.HandleAsync(callbackEvent);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateConferenceStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ConferenceState == ConferenceState.Suspended)), Times.Once);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.Body == "Technical assistance")), Times.Once);
        }
    }
}
