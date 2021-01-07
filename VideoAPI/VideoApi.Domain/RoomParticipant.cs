using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class RoomParticipant : Entity<long>
    {
        public RoomParticipant(long roomId, Guid participantId)
        {
            RoomId = roomId;
            ParticipantId = participantId;
        }

        public long RoomId { get; set; }
        public Guid ParticipantId { get; set; }
    }
}
