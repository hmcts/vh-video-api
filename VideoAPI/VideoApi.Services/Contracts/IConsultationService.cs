using System;
using System.Threading.Tasks;
using VideoApi.Domain.Enums;
using VideoApi.Services.Kinly;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId, CreateConsultationRoomParams createConsultationRoomParams);

        Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom);
    }
}
