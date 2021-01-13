using System;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {

        /// <summary>
        /// Calls Kinly endpoint to transfer participants between rooms.
        /// </summary>
        /// <param name="conferenceId">The conference UUID</param>
        /// <param name="participantId">The participant UUID</param>
        /// <param name="fromRoom">virtual court room</param>
        /// <param name="toRoom">virtual court room</param>
        /// <returns></returns>
        Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom,
            string toRoom);
    }
}
