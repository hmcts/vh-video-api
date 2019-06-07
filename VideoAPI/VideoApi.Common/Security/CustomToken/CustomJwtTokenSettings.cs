namespace VideoApi.Common.Security.CustomToken
{
    public class CustomJwtTokenSettings
    {
    public string Secret { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}