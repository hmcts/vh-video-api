using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public static class MeetingRoomToResponseMapper
    {
        public static MeetingRoomResponse MapVirtualCourtToResponse(MeetingRoom meetingRoom)
        {
            if (meetingRoom == null) return null;

            return new MeetingRoomResponse
            {
                AdminUri = meetingRoom.AdminUri,
                JudgeUri = meetingRoom.JudgeUri,
                ParticipantUri = meetingRoom.ParticipantUri,
                PexipNode = meetingRoom.PexipNode
            };
        }
    }
}
