namespace VideoApi.Common.Security.Kinly
{
    public interface ICustomJwtTokenProvider
    {
        string GenerateApiToken(string claims, int expiresInMinutes);
        string GenerateSelfTestApiToken(string claims, int expiresInMinutes);
        string GenerateTokenForCallbackEndpoint(string claims, int expiresInMinutes);
    }
}
