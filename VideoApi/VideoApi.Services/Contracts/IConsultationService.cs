using System;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Task<ConsultationRoom> GetAvailableConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType);

        Task EndpointTransferToRoomAsync(Guid conferenceId, Guid endpointId, string room);

        Task ParticipantTransferToRoomAsync(Guid conferenceId, Guid participantId, string room);

        Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom);

        Task<ConsultationRoom> CreateNewConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType = VirtualCourtRoomType.Participant, bool locked = true);
    }
}
