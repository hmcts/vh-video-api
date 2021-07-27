using VideoApi.Domain.Enums;

namespace VideoApi.Common.Security
{
    public interface IMagicLinksJwtTokenProvider
    {
        MagicLinksJwtDetails GenerateToken(string name, string userName, UserRole role);
    }
}
