using System.ComponentModel;

namespace VideoApi.Domain.Enums
{
    public enum QuickLinkHearingRole
    {
        [Description("Quick link participant")]
        QuickLinkParticipant = 8,
        [Description("Quick link observer")]
        QuickLinkObserver = 9
    }
}
