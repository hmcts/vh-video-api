using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class ConferenceStatus : Entity<long>
    {
        public ConferenceStatus(ConferenceState conferenceState)
        {
            ConferenceState = conferenceState;
            TimeStamp = DateTime.UtcNow;
        }

        public ConferenceState ConferenceState { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}