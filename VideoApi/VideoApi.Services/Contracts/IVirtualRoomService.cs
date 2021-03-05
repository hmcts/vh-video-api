using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.Services.Contracts
{
    public interface IVirtualRoomService
    {
        Task<Room> GetOrCreateAnInterpreterVirtualRoom(Conference conference, Participant participant);
        Task<Room> GetOrCreateAWitnessVirtualRoom(Conference conference, Participant participant);
    }
}
