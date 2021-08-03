using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class MagicLinkParticipant : ParticipantBase
    {
        public ParticipantToken Token { get; set; }

        public const string DOMAIN = "@magic-link-participant.com";

        public MagicLinkParticipant(string name, UserRole userRole)
        {
            Id = Guid.NewGuid();
            ParticipantRefId = Id;
            DisplayName = name;
            Username = $"{Id}{DOMAIN}";
            UserRole = userRole;
            Name = name;
        }
    }
}
