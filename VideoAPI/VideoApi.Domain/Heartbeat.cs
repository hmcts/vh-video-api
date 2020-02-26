using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class Heartbeat : Entity<long>
    {
        public Guid ConferenceId { get; set; }
        public Guid ParticipantId { get; set; }
        public decimal OutgoingAudioPercentageLost { get; set; }
        public decimal OutgoingAudioPercentageLostRecent { get; set; }
        public decimal IncomingAudioPercentageLost { get; set; }
        public decimal IncomingAudioPercentageLostRecent { get; set; }
        public decimal OutgoingVideoPercentageLost { get; set; }
        public decimal OutgoingVideoPercentageLostRecent { get; set; }
        public decimal IncomingVideoPercentageLost { get; set; }
        public decimal IncomingVideoPercentageLostRecent { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public DateTime Timestamp { get; set; }

        public Heartbeat(Guid conferenceId, Guid participantId, decimal outgoingAudioPercentageLost, decimal outgoingAudioPercentageLostRecent,
            decimal incomingAudioPercentageLost, decimal incomingAudioPercentageLostRecent, decimal outgoingVideoPercentageLost, 
            decimal outgoingVideoPercentageLostRecent, decimal incomingVideoPercentageLost, decimal incomingVideoPercentageLostRecent,
            DateTime timestamp, string browserName, string browserVersion)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            OutgoingAudioPercentageLost = outgoingAudioPercentageLost;
            OutgoingAudioPercentageLostRecent = outgoingAudioPercentageLostRecent;
            IncomingAudioPercentageLost = incomingAudioPercentageLost;
            IncomingAudioPercentageLostRecent = incomingAudioPercentageLostRecent;
            OutgoingVideoPercentageLost = outgoingVideoPercentageLost;
            OutgoingVideoPercentageLostRecent = outgoingVideoPercentageLostRecent;
            IncomingVideoPercentageLost = incomingVideoPercentageLost;
            IncomingVideoPercentageLostRecent = incomingVideoPercentageLostRecent;
            Timestamp = timestamp;
            BrowserName = browserName;
            BrowserVersion = browserVersion;
        }
    }
}
