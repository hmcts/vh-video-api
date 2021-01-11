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
            var roomParticipant = new RoomParticipant(participantId);
            roomParticipant.ParticipantId.Should().Be(participantId);
        }
    }
}
