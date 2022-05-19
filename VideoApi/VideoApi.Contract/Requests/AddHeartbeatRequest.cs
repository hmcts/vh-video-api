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
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public int IncomingVideoPacketsLost { get; set; }
        public int IncomingVideoPacketReceived { get; set; }
        public string IncomingVideoResolution { get; set; }
        public string IncomingVideoCodec { get; set; }
        public string IncomingVideoBitrate { get; set; }
        public int IncomingAudioPacketsLost { get; set; }
        public int IncomingAudioPacketReceived { get; set; }
        public string IncomingAudioCodec { get; set; }
        public string IncomingAudioBitrate { get; set; }
        public string OutgoingVideoResolution { get; set; }
        public string OutgoingVideoCodec { get; set; }
        public string OutgoingVideoBitrate { get; set; }
        public int OutgoingVideoFramerate { get; set; }
        public int OutgoingVideoPacketsLost { get; set; }
        public int OutgoingVideoPacketSent { get; set; }
        public int OutgoingAudioPacketSent { get; set; }
        public string OutgoingAudioCodec { get; set; }
        public string OutgoingAudioBitrate { get; set; }
        public int OutgoingAudioPacketsLost { get; set; }
    }
}
