using System;
using System.Collections.Generic;
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
    public class RoomParticipantJoinedEventHandlerTests : EventHandlerTestBase<RoomParticipantJoinedEventHandler>
    {
        [Test]
        public async Task Should_add_participant_to_room()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.UserRole == UserRole.Individual);
            var room = conference.Rooms.First();
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.RoomParticipantJoined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                ParticipantRoomId = conference.Rooms.First().Id,
                TimeStampUtc = DateTime.UtcNow
            };
            
            await _sut.HandleAsync(callbackEvent);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                    command.ConferenceId == conference.Id &&
                    command.ParticipantId == participantForEvent.Id &&
                    command.ParticipantState == ParticipantState.Available &&
                    command.Room == RoomType.WaitingRoom &&
                    command.RoomLabel == null)), Times.Once);
            
            CommandHandlerMock.Verify(
                x => x.Handle(It.Is<AddParticipantToParticipantRoomCommand>(command =>
                    command.ParticipantId == participantForEvent.Id && 
                    command.RoomId == room.Id)));
        }

        [Test]
        public async Task Should_add_participant_and_their_available_linked_participants_to_room()
        {
            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First(x => x.LinkedParticipants.Count > 0);
            var room = conference.Rooms.First();

            var participantIds = new List<Guid>
            {
                participantForEvent.Id
            };
            
            participantIds.AddRange(participantForEvent.LinkedParticipants
                .Select(linkedParticipant => linkedParticipant.Participant.Id));
            
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.RoomParticipantJoined,
                EventId = Guid.NewGuid().ToString(),
                ConferenceId = conference.Id,
                ParticipantId = participantForEvent.Id,
                ParticipantRoomId = conference.Rooms.First().Id,
                TimeStampUtc = DateTime.UtcNow
            };
            
            await _sut.HandleAsync(callbackEvent);

            foreach (var participantId in participantIds)
            {
                CommandHandlerMock.Verify(
                    x => x.Handle(It.Is<UpdateParticipantStatusAndRoomCommand>(command =>
                        command.ConferenceId == conference.Id &&
                        command.ParticipantId == participantId &&
                        command.ParticipantState == ParticipantState.Available &&
                        command.Room == RoomType.WaitingRoom &&
                        command.RoomLabel == null)), Times.Once);

                CommandHandlerMock.Verify(
                    x => x.Handle(It.Is<AddParticipantToParticipantRoomCommand>(command =>
                        command.ParticipantId == participantId &&
                        command.RoomId == room.Id)));
            }
        }
    }
}
