using System;

namespace VideoApi.Domain
{
    public class RoomParticipant : TrackableEntity<long>
    {
        public RoomParticipant(Guid participantId)
        {
            ParticipantId = participantId;
        }
        public Guid ParticipantId { get; private set; }
        public virtual ParticipantBase Participant { get; set; }
        public long RoomId { get; set; }
        
        public virtual Room Room { get; set; }
    }
}
