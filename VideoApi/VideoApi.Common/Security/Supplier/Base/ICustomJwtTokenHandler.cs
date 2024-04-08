using System.Security.Claims;

namespace VideoApi.Common.Security.Supplier.Base;

public interface ICustomJwtTokenHandler
{
    ClaimsPrincipal GetPrincipal(string token);
    bool IsValidToken(string token);
}
