using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public static class MeetingRoomToResponseMapper
    {
        public static MeetingRoomResponse MapVirtualCourtToResponse(MeetingRoom meetingRoom) => meetingRoom == null
                ? null
                : new MeetingRoomResponse
                {
                    AdminUri = meetingRoom.AdminUri,
                    JudgeUri = meetingRoom.JudgeUri,
                    ParticipantUri = meetingRoom.ParticipantUri,
                    PexipNode = meetingRoom.PexipNode,
                    TelephoneConferenceId = meetingRoom.TelephoneConferenceId
                };
    }
}
