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
    }
}
