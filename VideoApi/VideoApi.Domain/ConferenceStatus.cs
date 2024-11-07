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

        public ConferenceStatus(ConferenceState conferenceState, DateTime? timeStamp)
        {
            ConferenceState = conferenceState;
            if (timeStamp != null) TimeStamp = (DateTime)timeStamp;
        }

        public Guid ConferenceId { get; set; }
        public ConferenceState ConferenceState { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
