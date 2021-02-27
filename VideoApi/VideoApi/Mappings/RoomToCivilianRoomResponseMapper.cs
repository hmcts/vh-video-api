using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class RoomToCivilianRoomResponseMapper
    {
        public static CivilianRoomResponse MapToResponse(Room room)
        {
            return new CivilianRoomResponse
            {
                Id = room.Id,
                Label = room.Label,
                Participants = room.RoomParticipants.Select(x => x.ParticipantId).ToList()
            };
        }
    }
}
