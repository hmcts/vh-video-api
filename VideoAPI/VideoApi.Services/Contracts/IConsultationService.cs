using System;
using VideoApi.Domain.Enums;
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
        /// <param name="fromRoom">consultation room</param>
        /// <param name="toRoom">consultation room</param>
        /// <returns></returns>
        Task TransferParticipantAsync(Guid conferenceId, Guid participantId, VirtualCourtRoomType fromRoom,
            VirtualCourtRoomType toRoom);

        /// <summary>
        /// Returns participants from consultation room to waiting room.
        /// </summary>
        /// <param name="conferenceId">The conference UUID</param>
        /// <param name="participantId">The participant UUID</param>
        /// <param name="consultation">The consultation room</param>
        /// <returns></returns>
        Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, VirtualCourtRoomType consultation);
    }
}
