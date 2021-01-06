using FluentAssertions;
using NUnit.Framework;
using System;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Domain
{
    public class RoomParticipantTest
    {
        [Test]
        public void Should_create_roomParticipant()
        {
            var participantId = Guid.NewGuid();
            var roomId = 1;
            var date = DateTime.UtcNow;
            var roomParticipant = new RoomParticipant(roomId, participantId, date);
            roomParticipant.RoomId.Should().Be(roomId);
            roomParticipant.ParticipantId.Should().Be(participantId);
            roomParticipant.EnterTime.Should().Be(date);
        }

        [Test]
        public void Should_assign_leaved_time()
        {
            var leaveTime = DateTime.UtcNow;
            var roomParticipant = new RoomParticipant(1, Guid.NewGuid(), DateTime.UtcNow);
            roomParticipant.AssignLeaveTime(leaveTime);
            roomParticipant.LeaveTime.Should().Be(leaveTime);
        }
    }
}
