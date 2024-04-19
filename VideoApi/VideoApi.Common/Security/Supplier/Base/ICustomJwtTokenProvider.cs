namespace VideoApi.Common.Security.Supplier.Base;

public interface ICustomJwtTokenProvider
{
    string GenerateApiToken(string claims, int expiresInMinutes);
    string GenerateSelfTestApiToken(string claims, int expiresInMinutes);
    string GenerateTokenForCallbackEndpoint(string claims, int expiresInMinutes);
}
