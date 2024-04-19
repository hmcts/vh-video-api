namespace VideoApi.Common.Security.Supplier.Base
{
    public abstract class SupplierConfiguration
    {
        public string ApiSecret { get; set; }
        public string SelfTestApiSecret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string CallbackSecret { get; set; }
        public string ConferencePhoneNumber { get; set; }
        public string ConferencePhoneNumberWelsh { get; set; }
        public string ApiUrl { get; set; }
        public string SelfTestApiUrl { get; set; }
        public string CallbackUri { get; set; }
        public string PexipNode { get; set; }
        public string PexipSelfTestNode { get; set; }
        public string ConferenceUsername { get; set; }
    }
}
