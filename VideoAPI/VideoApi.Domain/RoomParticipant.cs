using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class RoomParticipant : Entity<long>
    {
        public RoomParticipant(long roomId, Guid participantId, DateTime enterTime)
        {
            RoomId = roomId;
            ParticipantId = participantId;
            EnterTime = enterTime;
        }

        public long RoomId { get; set; }
        public Guid ParticipantId { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime? LeaveTime { get; set; }

        public void AssignLeaveTime(DateTime leaveTime)
        {
            LeaveTime = leaveTime;
        }
    }


}
