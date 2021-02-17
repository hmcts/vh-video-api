using System;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class UpdateCurrentVirtualRoomTests
    {
        [Test]
        public void Should_update_current_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();

            participant.CurrentVirtualRoom.Should().BeNull();

            var newRoom = new Room(Guid.NewGuid(), "TestRoom1", VirtualCourtRoomType.JudgeJOH);
            participant.UpdateCurrentVirtualRoom(newRoom);

            participant.CurrentVirtualRoom.Should().Be(newRoom);
        }
    }
    public class UpdateCurrentRoomTests
    {
        [Test]
        public void Should_update_current_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();

            participant.CurrentRoom.Should().BeNull();

            var newRoom = RoomType.ConsultationRoom1;
            participant.UpdateCurrentRoom(newRoom);

            participant.CurrentRoom.Should().Be(newRoom);
        }
    }
}
