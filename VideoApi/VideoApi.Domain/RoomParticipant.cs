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
    }
}
