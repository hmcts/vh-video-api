using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class RoomToCivilianRoomResponseMapper
    {
        public static CivilianRoomResponse MapToResponse(ParticipantRoom consultationRoom)
        {
            return new CivilianRoomResponse
            {
                Id = consultationRoom.Id,
                Label = consultationRoom.Label,
                Participants = consultationRoom.RoomParticipants.Select(x => x.ParticipantId).ToList(), 
                Status = consultationRoom.Status.MapToContractEnum()
            };
        }
    }
}
