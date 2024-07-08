using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Participants;

public class CanAutoTransferToHearingRoomTests
{
    [Test]
    public void Should_return_false_when_user_role_is_QuickLinkParticipant()
    {
        var participant = new ParticipantBuilder().WithUserRole(UserRole.QuickLinkParticipant).Build();
        var result = participant.CanAutoTransferToHearingRoom();
        result.Should().BeFalse();
    }
    
    [Test]
    public void Should_return_false_when_user_role_is_QuickLinkObserver()
    {
        var participant = new ParticipantBuilder().WithUserRole(UserRole.QuickLinkObserver).Build();
        var result = participant.CanAutoTransferToHearingRoom();
        result.Should().BeFalse();
    }

    [Test]
    public void Should_return_false_when_user_role_is_StaffMember()
    {
        var participant = new ParticipantBuilder().WithUserRole(UserRole.StaffMember).Build();
        var result = participant.CanAutoTransferToHearingRoom();
        result.Should().BeFalse();
    }
    
    [Test]
    public void Should_return_false_when_user_role_is_Witness()
    {
        var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual).WithHearingRole("Witness").Build();
        var result = participant.CanAutoTransferToHearingRoom();
        result.Should().BeFalse();
    }
    
    [Test]
    public void Should_return_true_when_user_role_is_Individual()
    {
        var participant = new ParticipantBuilder().WithUserRole(UserRole.Individual).Build();
        var result = participant.CanAutoTransferToHearingRoom();
        result.Should().BeTrue();
    }
    
}
