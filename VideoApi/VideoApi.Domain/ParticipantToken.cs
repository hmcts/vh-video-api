using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class ParticipantToken : Entity<Guid>
    {
        public string Jwt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid ParticipantId { get; set; }
        public QuickLinkParticipant Participant { get; set; }
    }
}
