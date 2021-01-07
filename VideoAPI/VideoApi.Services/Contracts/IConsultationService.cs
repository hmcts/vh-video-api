using System;
using System.Threading.Tasks;
using VideoApi.Domain.Enums;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Task StartConsultationAsync(Guid conferenceId, Guid requestedBy, RoomType roomType);

        Task TransferParticipantAsync(Guid conferenceId, Guid participantId, RoomType fromRoom, RoomType toRoom);
    }
}
