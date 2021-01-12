using FluentAssertions;
using NUnit.Framework;
using System;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain
{
    public class RoomTest
    {
        [Test]
        public void Should_create_room_and_set_status_to_created()
        {
            var conferenceId = Guid.NewGuid();
            var label = "Room1";

            var room = new Room(conferenceId, label, VirtualCourtRoomType.JudgeJOH);
            room.Status.Should().Be(RoomStatus.Live);
            room.ConferenceId.Should().Be(conferenceId);
            room.Label.Should().Be(label);
            room.Type.Should().Be(VirtualCourtRoomType.JudgeJOH);
        }

        [Test]
        public void Should_add_participant_to_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().Be(1);
            room.RoomParticipants[0].ParticipantId.Should().Be(participantId);
        }

        [Test]
        public void Should_not_add_existing_participant_to_room_and_throw_exception()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

            Action action = () => room.AddParticipant(roomParticipant);

            action.Should().Throw<DomainRuleException>();
            room.RoomParticipants.Count.Should().Be(beforeCount);
        }

        [Test]
        public void Should_remove_participant_from_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

            room.RemoveParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().BeLessThan(beforeCount);
        }

        [Test]
        public void Should_throw_exception_for_remove_non_existing_participant_from_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

           Action action = () => room.RemoveParticipant(new RoomParticipant(Guid.NewGuid()));

            action.Should().Throw<DomainRuleException>();
            room.RoomParticipants.Count.Should().Be(beforeCount);
        }

        [Test]
        public void Should_update_room_status_to_Live_on_init()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.Status.Should().Be(RoomStatus.Live);
        }

        [Test]
        public void Should_update_room_status_to_closed_on_last_participant_remove()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            var roomParticipant = new RoomParticipant(Guid.NewGuid());
            room.RoomParticipants.Add(roomParticipant);
            room.Status.Should().Be(RoomStatus.Live);

            room.RemoveParticipant(roomParticipant);

            room.Status.Should().Be(RoomStatus.Closed);
        }

        [Test]
        public void Should_return_list_of_participant_in_a_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(1, participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().Be(1);
            room.RoomParticipants[0].ParticipantId.Should().Be(participantId);

            var participantsList = room.GetRoomParticipants();
            participantsList.Count.Should().Be(1);
        }
    }
}
