namespace VideoApi.Common.Security.CustomToken
{
    public interface ICustomJwtTokenSettings
    {
        int ExpiresInMinutes { get; }
        string Secret { get; }
        string Audience { get; }
        string Issuer { get; }
    }
}