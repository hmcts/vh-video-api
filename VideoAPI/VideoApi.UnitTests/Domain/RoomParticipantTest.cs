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
            var roomParticipant = new RoomParticipant(roomId, participantId);
            roomParticipant.RoomId.Should().Be(roomId);
            roomParticipant.ParticipantId.Should().Be(participantId);
            roomParticipant.EnterTime.Ticks.Should().BeGreaterOrEqualTo(date.Ticks);
        }
    
    }
}
