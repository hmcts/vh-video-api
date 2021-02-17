using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class GetCurrentRoomTests
    {
        [TestCase(RoomType.WaitingRoom)]
        [TestCase(RoomType.HearingRoom)]
        [TestCase(RoomType.ConsultationRoom1)]
        [TestCase(RoomType.ConsultationRoom2)]
        [TestCase(RoomType.AdminRoom)]
        public void should_get_current_room(RoomType newRoom)
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();
            participant.UpdateCurrentRoom(newRoom);
            participant.GetCurrentRoom().Should().Be(newRoom);
        }

        [Test]
        public void should_throw_exception_when_endpoint_is_not_in_a_room()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithCaseTypeGroup("Applicant")
                .Build();
            participant.UpdateCurrentRoom(null);
            Assert.Throws<DomainRuleException>(() => participant.GetCurrentRoom()).ValidationFailures
                .Any(x => x.Message == "Participant is not in a room").Should().BeTrue();
        }
    }
}
