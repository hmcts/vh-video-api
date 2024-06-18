using FluentAssertions;
using VideoApi.Contract.Responses;

namespace Testing.Common.Assertions
{
    public static class AssertParticipantResponse
    {
        public static void ForParticipant(ParticipantResponse participant)
        {
            participant.Should().NotBeNull("Participant is null");
            participant.Id.Should().NotBeEmpty($"Id is '{participant.Id}'");
            participant.Username.Should().NotBeNullOrWhiteSpace($"Username is '{participant.Username}'");
            participant.UserRole.ToString().Should().NotBeNullOrWhiteSpace($"UserRole is '{participant.UserRole}'");
            participant.DisplayName.Should().NotBeNullOrWhiteSpace($"DisplayName is '{participant.DisplayName}'");
        }
    }
}
