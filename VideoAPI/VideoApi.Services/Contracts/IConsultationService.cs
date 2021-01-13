using VideoApi.Contract.Requests;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {


        /// <summary>
        /// Transfers a participant from the consultation room to the waiting room.
        /// </summary>
        /// <param name="conferenceId">The conference UUID</param>
        /// <param name="participantId">The participant UUID</param>
        /// <param name="fromRoom">virtual court room</param>
        /// <param name="toRoom">virtual court room</param>
        /// <returns></returns>
        Task LeaveConsultationAsync(LeaveConsultationRequest request, string fromRoom, string toRoom);
    }
}
