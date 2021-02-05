using System;

namespace VideoApi.Contract.Responses
{
    public class ParticipantHeartbeatResponse
    {
        public decimal RecentPacketLoss { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
