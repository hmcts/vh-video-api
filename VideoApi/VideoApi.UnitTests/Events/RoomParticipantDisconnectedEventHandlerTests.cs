using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Events
{
    public class
        RoomParticipantDisconnectedEventHandlerTests : EventHandlerTestBase<RoomParticipantDisconnectedEventHandler>
    {
        [Test]
        public async Task should_move_vmr_back_to_waiting_room_when_room_participant_disconnects_during_a_consultation()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants()
                .First(x => x.UserRole == UserRole.Individual && x.LinkedParticipants.Any());
            var interpreterRoomId = conference.Rooms.OfType<InterpreterRoom>().First().Id;
            var consultationRoom = new ConsultationRoom(conference.Id, "ConsultationRoom1",
                VirtualCourtRoomType.Participant, false);
            participantForEvent.State = ParticipantState.InConsultation;
            participantForEvent.UpdateCurrentConsultationRoom(consultationRoom);
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.RoomParticipantDisconnected,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id,
                ParticipantRoomId = interpreterRoomId,
                ConferenceId = conference.Id,
                Reason = "Unexpected drop",
                TimeStampUtc = DateTime.UtcNow
            };
            
            await _sut.HandleAsync(callbackEvent);

            _mocker.Mock<IConsultationService>()
                .Verify(
                    x => x.LeaveConsultationAsync(conference.Id, participantForEvent.Id, consultationRoom.Label,
                        RoomType.WaitingRoom.ToString()), Times.Once());
            
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
            
        }
    }
}
