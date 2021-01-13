using Threading = System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Domain;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {
        Threading.Task<Room> GetAvailableConsultationRoomAsync(StartConsultationRequest request);

        Threading.Task TransferParticipantToConsultationRoomAsync(StartConsultationRequest request, Room room);
    }
}
