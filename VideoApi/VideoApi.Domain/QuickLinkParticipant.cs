using System;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Consts;

namespace VideoApi.Domain
{
    public class QuickLinkParticipant : ParticipantBase
    {
        public ParticipantToken Token { get; set; }

        public const string DOMAIN = "@quick-link-participant.com";

        public QuickLinkParticipant(string name, UserRole userRole) 
        {
            Id = Guid.NewGuid();
            ParticipantRefId = Id;
            DisplayName = name;
            Username = $"{Id}{DOMAIN}";
            UserRole = userRole;
            Name = name;
            HearingRole = ((QuickLinkHearingRole)Enum.Parse(typeof(UserRole), userRole.ToString())).ToString();
        }
    }
}
