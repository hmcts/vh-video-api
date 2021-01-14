using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Task<Room> GetAvailableConsultationRoomAsync(StartConsultationRequest request);

        Task TransferParticipantToConsultationRoomAsync(StartConsultationRequest request, Room room);

        Task LeaveConsultationAsync(LeaveConsultationRequest request, string fromRoom, string toRoom);
    }
}
