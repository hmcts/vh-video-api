using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class RoomToDetailsResponseMapper
    {
        public static RoomResponse MapRoomToResponse(ConsultationRoom consultationRoom)
        {
            if (consultationRoom == null)
            {
                return null;
            }

            return new RoomResponse
            {
                Id = consultationRoom.Id,
                Label = consultationRoom.Label,
                Locked = consultationRoom.Locked
            };
        }
    }
}
