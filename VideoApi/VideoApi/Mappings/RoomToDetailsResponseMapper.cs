using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class RoomToDetailsResponseMapper
    {
        public static RoomResponse MapConsultationRoomToResponse(Room room)
        {
            if (room == null)
            {
                return null;
            }

            return new RoomResponse
            {
                Id = room.Id,
                Label = room.Label,
                Locked = room.Locked
            };
        }
    }
}
