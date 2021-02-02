using System.Security.Claims;

namespace VideoApi.Common.Security.Kinly
{
    public interface ICustomJwtTokenHandler
    {
        ClaimsPrincipal GetPrincipal(string token);
        bool IsValidToken(string token);
    }
}