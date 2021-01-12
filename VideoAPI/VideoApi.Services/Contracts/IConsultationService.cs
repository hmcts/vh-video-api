using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoApi.Contract.Requests;
using VideoApi.Domain;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Task<Room> GetAvailableConsultationRoomAsync(StartConsultationRequest request);

        Task<IActionResult> TransferParticipantToConsultationRoomAsync(StartConsultationRequest request, Room room);
    }
}
