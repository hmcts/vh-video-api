using System;
using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.Services
{
    public interface IVideoPlatformService
    {
        Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId);
        Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId);
    }
}