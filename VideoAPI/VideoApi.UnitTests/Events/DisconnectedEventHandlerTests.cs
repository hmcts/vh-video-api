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
    public class DisconnectedEventHandlerTests : EventHandlerTestBase<DisconnectedEventHandler>
    {
        [Test]
        public async Task Should_send_disconnect_messages_to_participants_and_service_bus_on_participant_disconnect()
        {
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
            var updateStatusCommand = new UpdateParticipantStatusAndRoomCommand(conference.Id, participantForEvent.Id,
                ParticipantState.Disconnected, null);
            CommandHandlerMock.Setup(x => x.Handle(updateStatusCommand));

            var addParticipantDisconnectedTask =
                new AddTaskCommand(conference.Id, conference.Id, "Disconnected", TaskType.Participant);
            CommandHandlerMock.Setup(x => x.Handle(addParticipantDisconnectedTask));
            
            await _sut.HandleAsync(callbackEvent);


            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Disconnected &&
                    command.Room == null)), Times.Once);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.OriginId == participantForEvent.Id &&
                    command.TaskType == TaskType.Participant)), Times.Once);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.OriginId == conference.Id &&
                    command.TaskType == TaskType.Hearing)), Times.Never);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == participantForEvent.Id &&
                    command.OriginId == participantForEvent.Id &&
                    command.TaskType == TaskType.Judge)), Times.Never);
        }
    }
}
