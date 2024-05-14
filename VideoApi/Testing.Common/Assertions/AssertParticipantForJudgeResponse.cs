using FluentAssertions;
using VideoApi.Contract.Responses;
using VideoApi.Contract.Enums;

namespace Testing.Common.Assertions
{
    public static class AssertParticipantForJudgeResponse
    {
        public static void ForParticipant(ParticipantForHostResponse participant)
        {
            participant.Should().NotBeNull();
            participant.Role.Should().Be(participant.Role);
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
