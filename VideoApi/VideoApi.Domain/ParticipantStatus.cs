using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class ParticipantStatus : Entity<long>
    {
        public ParticipantStatus(ParticipantState participantState)
        {
            ParticipantState = participantState;
            TimeStamp = DateTime.UtcNow;
        }

        public ParticipantState ParticipantState { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
