namespace VideoApi.Common.Security.CustomToken
{
    public interface ICustomJwtTokenProvider
    {
        string GenerateToken(string basedOn, int expiresInMinutes);
    }
}