using System;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Validations;

namespace VideoApi.Services
{
    public interface IVideoPlatformService
    {
        Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId);
        Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId);
        Task<TestCallResult> GetTestCallScoreAsync(Guid participantId);
    }
}