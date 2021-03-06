using System;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class SharedParticipantRoomResponseMapper
    {
        public static SharedParticipantRoomResponse MapRoomToResponse(Room room)
        {
            return new SharedParticipantRoomResponse
            {
                Label = room.Label,
                ParticipantJoinUri = room.ParticipantUri,
                PexipNode = room.PexipNode,
                RoomType = Enum.Parse<VirtualCourtRoomType>(room.Type.ToString(), true)
            };
        }
    }
}
