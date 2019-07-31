using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class DisconnectedEventHandlerTests : EventHandlerTestBase
    {
        private DisconnectedEventHandler _eventHandler;

        [Test]
        public async Task should_send_disconnect_messages_to_participants_and_service_bus_on_participant_disconnect()
        {
            _eventHandler = new DisconnectedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient, EventHubContextMock.Object);

            var conference = TestConference;
            var participantCount = conference.GetParticipants().Count + 1; // plus one for admin
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
            
            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(participantForEvent.Username, ParticipantState.Disconnected),
                Times.Exactly(participantCount));

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

        [Test]
        public async Task
            should_send_disconnect_and_suspend_messages_to_participants_and_service_bus_on_judge_disconnect()
        {
            _eventHandler = new DisconnectedEventHandler(QueryHandlerMock.Object, CommandHandlerMock.Object,
                ServiceBusQueueClient, EventHubContextMock.Object);

            var conference = TestConference;
            var participantCount = conference.GetParticipants().Count + 1; // plus one for admin
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Judge);
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Disconnected,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ConferenceId = conference.Id,
                TimeStampUtc = DateTime.UtcNow
            };
            var updateParticipantStatusCommand = new UpdateParticipantStatusAndRoomCommand(conference.Id,
                participantForEvent.Id,
                ParticipantState.Disconnected,
                null);
            CommandHandlerMock.Setup(x => x.Handle(updateParticipantStatusCommand));

            var updateConferenceStatusCommand =
                new UpdateConferenceStatusCommand(conference.Id, ConferenceState.Suspended);
            CommandHandlerMock.Setup(x => x.Handle(updateConferenceStatusCommand));

            var hearingSuspendedTask = new AddTaskCommand(conference.Id, conference.Id, "Suspended", TaskType.Hearing);
            CommandHandlerMock.Setup(x => x.Handle(hearingSuspendedTask));
            
            var addJudgeDisconnectedTask =
                new AddTaskCommand(conference.Id, conference.Id, "Disconnected", TaskType.Judge);
            CommandHandlerMock.Setup(x => x.Handle(addJudgeDisconnectedTask));

            await _eventHandler.HandleAsync(callbackEvent);
            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.ParticipantStatusMessage(_eventHandler.SourceParticipant.Username,
                    ParticipantState.Disconnected),
                Times.Exactly(participantCount));

            EventHubClientMock.Verify(
                x => x.ConferenceStatusMessage(conference.Id, ConferenceState.Suspended),
                Times.Exactly(participantCount));

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Disconnected &&
                    command.Room == null)), Times.Once);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateConferenceStatusCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ConferenceState == ConferenceState.Suspended)), Times.Once);

            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.OriginId == participantForEvent.Id &&
                    command.TaskType == TaskType.Participant)), Times.Never);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.OriginId == conference.Id &&
                    command.TaskType == TaskType.Hearing)), Times.Once);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddTaskCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.OriginId == participantForEvent.Id &&
                    command.TaskType == TaskType.Judge)), Times.Once);
        }
    }
}