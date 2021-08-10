using VideoApi.Domain.Enums;

namespace VideoApi.Common.Security
{
    public interface IQuickLinksJwtTokenProvider
    {
        QuickLinksJwtDetails GenerateToken(string name, string userName, UserRole role);
    }
}
