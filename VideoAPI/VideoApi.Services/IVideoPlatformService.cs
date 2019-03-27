using System;
using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.Services
{
    public interface IVideoPlatformService
    {
        Task<MeetingRoom> BookVirtualCourtroom(Guid conferenceId);
    }
}