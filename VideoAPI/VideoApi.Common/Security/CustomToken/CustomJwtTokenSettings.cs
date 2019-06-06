namespace VideoApi.Common.Security.CustomToken
{
    public class CustomJwtTokenSettings : ICustomJwtTokenSettings
    {
        public CustomJwtTokenSettings(int expiresInMinutes, string secret, string audience, string issuer)
        {
            ExpiresInMinutes = expiresInMinutes;
            Secret = secret;
            Audience = audience;
            Issuer = issuer;
        }

        public int ExpiresInMinutes { get; }
        public string Secret { get; }
        public string Audience { get; }
        public string Issuer { get; }
    }
}