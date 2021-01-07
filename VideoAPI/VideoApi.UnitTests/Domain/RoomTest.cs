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
            room.Status.Should().Be(RoomStatus.Created);
            room.ConferenceId.Should().Be(conferenceId);
            room.Label.Should().Be(label);
            room.Type.Should().Be(VirtualCourtRoomType.JudgeJOH);
        }

        [Test]
        public void Should_add_participant_to_room()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(1, participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);

            room.RoomParticipants.Count.Should().Be(1);
            room.RoomParticipants[0].ParticipantId.Should().Be(participantId);
        }

        [Test]
        public void Should_not_add_existing_participant_to_room_and_throw_exception()
        {
            var participantId = Guid.NewGuid();
            var roomParticipant = new RoomParticipant(1, participantId);
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
            var roomParticipant = new RoomParticipant(1, participantId);
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
            var roomParticipant = new RoomParticipant(1, participantId);
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);
            room.AddParticipant(roomParticipant);
            var beforeCount = room.RoomParticipants.Count;

           Action action = () => room.RemoveParticipant(new RoomParticipant(1, Guid.NewGuid()));

            action.Should().Throw<DomainRuleException>();
            room.RoomParticipants.Count.Should().Be(beforeCount);
        }

        [Test]
        public void Should_update_room_status()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);

            room.UpdateStatus(RoomStatus.Live);

            room.Status.Should().Be(RoomStatus.Live);
        }

        [Test]
        public void Should_not_update_room_status_if_room_closed()
        {
            var room = new Room(Guid.NewGuid(), "Room1", VirtualCourtRoomType.JudgeJOH);

            room.UpdateStatus(RoomStatus.Closed);
            room.Status.Should().Be(RoomStatus.Closed);

            Action action = () => room.UpdateStatus(RoomStatus.Live);

            action.Should().Throw<DomainRuleException>();
            room.Status.Should().Be(RoomStatus.Closed);
        }
    }
}
