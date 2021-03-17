using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class RoomParticipant : Entity<long>
    {
        public RoomParticipant(Guid participantId)
        {
            ParticipantId = participantId;
        }
        public Guid ParticipantId { get; private set; }
        public virtual Participant Participant { get; set; }
        public long RoomId { get; set; }
        
        public virtual Room Room { get; set; }
    }
}
