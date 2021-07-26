using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class MagicLinkParticipant : ParticipantBase
    {
        public ParticipantToken Token { get; set; }

        public MagicLinkParticipant(string name, UserRole userRole)
        {
            Id = Guid.NewGuid();
            ParticipantRefId = Id;
            DisplayName = name;
            Username = $"{Id}@magic-link-participant.com";
            UserRole = userRole;
            Name = name;
        }
    }
}
