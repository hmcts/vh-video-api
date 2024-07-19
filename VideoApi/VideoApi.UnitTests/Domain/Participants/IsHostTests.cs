using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants;

public class IsHostTests
{
    [TestCase(UserRole.QuickLinkParticipant, false)]
    [TestCase(UserRole.QuickLinkObserver, false)]
    [TestCase(UserRole.Individual, false)]
    [TestCase(UserRole.Representative, false)]
    [TestCase(UserRole.JudicialOfficeHolder, false)]
    [TestCase(UserRole.Judge, true)]
    [TestCase(UserRole.StaffMember, true)]
    public void should_check_if_user_role_is_a_host(UserRole userRole, bool expected)
    {
        var participant = new ParticipantBuilder().WithUserRole(userRole).Build();
        var result = participant.IsHost();
        result.Should().Be(expected);
    }
}
