using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class UpdateParticipantStatusTests
    {
        [Test]
        public void Should_add_participant_status()
        {
            var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual)
                .WithHearingRole("Applicant")
                .Build();
            
            var beforeCount = participant.GetParticipantStatuses().Count;

            var participantStatus = ParticipantState.Joining;
            participant.UpdateParticipantStatus(participantStatus);
            var afterCount = participant.GetParticipantStatuses().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            participant.GetCurrentStatus().ParticipantState.Should().Be(participantStatus);
        }
    }
}
