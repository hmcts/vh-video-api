using System.Threading.Tasks;

namespace VideoApi.Services
{
    public interface IUserProfileService
    {
        Task<bool> IsAdmin(string username);
    }
}