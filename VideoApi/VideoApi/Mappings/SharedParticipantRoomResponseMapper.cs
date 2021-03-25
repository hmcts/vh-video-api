using System;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class SharedParticipantRoomResponseMapper
    {
        public static SharedParticipantRoomResponse MapRoomToResponse(ParticipantRoom consultationRoom)
        {
            return new SharedParticipantRoomResponse
            {
                Label = consultationRoom.Label,
                ParticipantJoinUri = consultationRoom.ParticipantUri,
                PexipNode = consultationRoom.PexipNode,
                RoomType = Enum.Parse<VirtualCourtRoomType>(consultationRoom.Type.ToString(), true)
            };
        }
    }
}
