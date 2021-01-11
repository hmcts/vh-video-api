using System;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IConsultationService
    {

        /// <summary>
        /// Returns participants from consultation room to waiting room.
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        Task EndJudgeJohConsultationAsync(Guid conferenceId, Room room);
    }
}
