using EnumsNET;
using System;
using VideoApi.Domain.Consts;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class QuickLinkParticipant : ParticipantBase
    {
        public ParticipantToken Token { get; set; }
        
        public QuickLinkParticipant(string name, UserRole userRole) 
        {
            Id = Guid.NewGuid();
            ParticipantRefId = Id;
            DisplayName = name;
            Username = $"{Id}{QuickLinkParticipantConst.Domain}";
            UserRole = userRole;
            Name = name;
            HearingRole = ((QuickLinkHearingRole)Enum.Parse(typeof(UserRole), userRole.ToString())).AsString(EnumFormat.Description);
        }
    }
}
