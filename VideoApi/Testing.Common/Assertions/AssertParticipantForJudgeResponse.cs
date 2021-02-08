using FluentAssertions;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace Testing.Common.Assertions
{
    public static class AssertParticipantForJudgeResponse
    {
        public static void ForParticipant(ParticipantForJudgeResponse participant)
        {
            participant.Should().NotBeNull();
            participant.Role.Should().NotBeNull();
            participant.DisplayName.Should().NotBeNullOrWhiteSpace();
            participant.CaseTypeGroup.Should().NotBeNullOrWhiteSpace();
            participant.HearingRole.Should().NotBeNullOrWhiteSpace();
            if (participant.Role == UserRole.Representative)
            {
                participant.Representee.Should().NotBeEmpty();
            }
        }
    }
}
