using System;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class GetInterpreterRoomTests
    {
        [Test]
        public void should_return_null_when_participant_is_not_in_any_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();

            participant.GetParticipantRoom().Should().BeNull();
        }
        
        [Test]
        public void should_return_null_when_participant_is_not_in_an_interpreter_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();
            var consultationRoom = new ConsultationRoom(Guid.NewGuid(), "ConsultationRoom1",
                VirtualCourtRoomType.Participant, false);
            
            consultationRoom.SetProtectedProperty(nameof(consultationRoom.Id), 999);
            var roomParticipant = new RoomParticipant(participant.Id)
            {
                Room = consultationRoom,
                RoomId = consultationRoom.Id
            };
            consultationRoom.AddParticipant(roomParticipant);
            participant.RoomParticipants.Add(roomParticipant);

            participant.GetParticipantRoom().Should().BeNull();
        }
        
        [Test]
        public void should_return_interpreter_when_participant_is_in_an_interpreter_room()
        {
            var conferenceId = Guid.NewGuid();
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();
            var consultationRoom = new ConsultationRoom(conferenceId, "ConsultationRoom1",
                VirtualCourtRoomType.Participant, false);
            consultationRoom.SetProtectedProperty(nameof(consultationRoom.Id), 998);
            
            var interpreterRoom = new ParticipantRoom(conferenceId, "Interpreter1", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(interpreterRoom.Id), 999);
            
            var consultationRoomParticipant = new RoomParticipant(participant.Id)
            {
                Room = consultationRoom,
                RoomId = consultationRoom.Id
            };
            var interpreterRoomParticipant = new RoomParticipant(participant.Id)
            {
                Room = interpreterRoom,
                RoomId = interpreterRoom.Id
            };
            consultationRoom.AddParticipant(consultationRoomParticipant);
            interpreterRoom.AddParticipant(interpreterRoomParticipant);
            participant.RoomParticipants.Add(interpreterRoomParticipant);
            participant.RoomParticipants.Add(consultationRoomParticipant);

            participant.GetParticipantRoom().Should().Be(interpreterRoom);
        }
    }
}
