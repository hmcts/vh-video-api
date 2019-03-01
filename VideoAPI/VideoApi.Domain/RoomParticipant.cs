using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain.Validations
{
    public class RoomParticipant : Entity<long>
    {

        public RoomParticipant(CourtRoom courtRoom, Participant participant)
        {
            CourtRoomId = courtRoom.Id;
            ParticipantId = participant.Id;
            EnterTime = DateTime.UtcNow;
        }

        public long CourtRoomId { get; set; }
        public long ParticipantId { get; set; }
        public DateTime EnterTime { get; private set; }
        public DateTime ExitTime { get; private set; }
        
        public void ExitRoom()
        {
            ExitTime = DateTime.UtcNow;
        }
    }
}