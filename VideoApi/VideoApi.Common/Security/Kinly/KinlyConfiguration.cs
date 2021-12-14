namespace VideoApi.Common.Security.Kinly
{
    public class KinlyConfiguration
    {
        public string ApiSecret { get; set; }
        public string SelfTestApiSecret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string CallbackSecret { get; set; }
        public string ConferencePhoneNumber { get; set; }
        public string ConferencePhoneNumberWelsh { get; set; }
        public string KinlyApiUrl { get; set; }
        public string KinlySelfTestApiUrl { get; set; }
        public string CallbackUri { get; set; }
        public string PexipNode { get; set; }
        public string PexipSelfTestNode { get; set; }
        public string ConferenceUsername { get; set; }
    }
}
