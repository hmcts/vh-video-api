using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Services.Kinly;

namespace VideoApi.Mappings
{
    public static class ConferenceInterpreterRoomsResponseMapper
    {
        public static List<SharedParticipantRoomResponse> Map(IList<Conference> conferences)
        {
            return conferences.SelectMany(x => x.Rooms.Where(s => s.Label.Contains(nameof(KinlyRoomType.Interpreter))),
                (conference, interpreterRoom) =>
                    new SharedParticipantRoomResponse()
                    {
                        HearingId = conference.HearingRefId.ToString(),
                        Label = interpreterRoom.Label
                    }).ToList();
        }
    }
}
