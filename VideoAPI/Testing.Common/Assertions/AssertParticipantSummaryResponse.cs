using FluentAssertions;
using VideoApi.Contract.Responses;

namespace Testing.Common.Assertions
{
    public static class AssertParticipantSummaryResponse
    {
        public static void ForParticipant(ParticipantSummaryResponse participant)
        {
            participant.Should().NotBeNull();
            participant.Username.Should().NotBeNullOrWhiteSpace();
            participant.UserRole.Should().NotBeNull();
            participant.DisplayName.Should().NotBeNullOrWhiteSpace();
        }
    }
}