using FluentAssertions;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace Testing.Common.Assertions
{
    public static class AssertParticipantSummaryResponse
    {
        public static void ForParticipant(ParticipantSummaryResponse participant)
        {
            participant.Should().NotBeNull("Participant is null");
            participant.Id.Should().NotBeEmpty($"Id is '{participant.Id}'");
            participant.Username.Should().NotBeNullOrWhiteSpace($"Username is '{participant.Username}'");
            participant.HearingRole.Should().NotBeNullOrWhiteSpace($"HearingRole is '{participant.HearingRole}'");
            participant.UserRole.ToString().Should().NotBeNullOrWhiteSpace($"UserRole is '{participant.UserRole}'");
            participant.DisplayName.Should().NotBeNullOrWhiteSpace($"DisplayName is '{participant.DisplayName}'");
            participant.FirstName.Should().NotBeNullOrWhiteSpace($"FirstName is '{participant.FirstName}'");
            participant.LastName.Should().NotBeNullOrWhiteSpace($"LastName is '{participant.LastName}'");
            participant.ContactEmail.Should().NotBeNullOrWhiteSpace($"ContactEmail is '{participant.ContactEmail}'");
        }
    }
}
