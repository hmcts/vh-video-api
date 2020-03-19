namespace VideoApi.Contract.Requests
{
    public class AddHeartbeatRequest
    {
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
    }
}
