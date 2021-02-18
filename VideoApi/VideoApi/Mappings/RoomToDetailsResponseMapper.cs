using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class RoomToDetailsResponseMapper
    {
        public static RoomResponse MapRoomToResponse(Room room)
        {
            if (room == null)
            {
                return null;
            }

            return new RoomResponse
            {
                Label = room.Label,
                Locked = room.Locked
            };
        }
    }
}
