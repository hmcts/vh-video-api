using System;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Task<Room> GetAvailableConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType);

        Task JoinConsultationRoomAsync(Guid conferenceId, Guid participantId, string room);

        Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom);
    }
}