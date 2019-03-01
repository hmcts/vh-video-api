using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class UpdateParticipantStatusTests
    {
        [Test]
        public void should_add_conference_status()
        {
            var participant = new ParticipantBuilder("Claimant LIP", "Claimant")
                .Build();

            participant.GetCurrentStatus().Should().BeNull();
            var beforeCount = participant.GetParticipantStatuses().Count;

            var participantStatus = ParticipantState.Joining; 
            participant.UpdateParticipantStatus(participantStatus);
            var afterCount = participant.GetParticipantStatuses().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            participant.GetCurrentStatus().ParticipantState.Should().Be(participantStatus);
        }
    }
}