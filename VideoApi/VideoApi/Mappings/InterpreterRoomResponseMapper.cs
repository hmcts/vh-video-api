using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class InterpreterRoomResponseMapper
    {
        public static InterpreterRoomResponse MapRoomToResponse(Room room)
        {
            return new InterpreterRoomResponse
            {
                Label = room.Label,
                ParticipantJoinUri = room.ParticipantUri,
                PexipNode = room.PexipNode
            };
        }
    }
}
