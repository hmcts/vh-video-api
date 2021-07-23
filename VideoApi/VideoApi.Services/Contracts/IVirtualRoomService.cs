using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.Services.Contracts
{
    public interface IVirtualRoomService
    {
        /// <summary>
        /// Create a room for linked participants.
        /// </summary>
        /// <param name="conference"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task<ParticipantRoom> GetOrCreateAnInterpreterVirtualRoom(Conference conference, ParticipantBase participant);
        
        /// <summary>
        /// Create a room for linked participants of type witness.
        /// This room, like witnesses, will not be pulled into a hearing when a hearing starts.
        /// A judge will transfer them in or out like normal participants.
        /// </summary>
        /// <param name="conference"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task<ParticipantRoom> GetOrCreateAWitnessVirtualRoom(Conference conference, ParticipantBase participant);
        
        /// <summary>
        /// Create a room for linked participants of type witness.
        /// This room, like witnesses, will not be pulled into a hearing when a hearing starts.
        /// A judge will transfer them in or out like normal participants.
        /// </summary>
        /// <param name="conference"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        Task<ParticipantRoom> GetOrCreateAJudicialVirtualRoom(Conference conference, ParticipantBase participant);
    }
}
