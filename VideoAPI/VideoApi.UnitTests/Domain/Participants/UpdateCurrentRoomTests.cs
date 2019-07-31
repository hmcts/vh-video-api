using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class UpdateCurrentRoomTests
    {
        [Test]
        public void should_update_current_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Claimant")
                .Build();

            participant.CurrentRoom.Should().BeNull();

            var newRoom = RoomType.ConsultationRoom1;
            participant.UpdateCurrentRoom(newRoom);

            participant.CurrentRoom.Should().Be(newRoom);
        }
    }
}