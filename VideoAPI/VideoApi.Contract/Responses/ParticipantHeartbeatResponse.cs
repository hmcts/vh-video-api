namespace VideoApi.Contract.Responses
{
    public class ParticipantHeartbeatResponse
    {
        public decimal RecentPacketLoss { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
    }
}
