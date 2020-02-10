using VideoApi.Contract.Responses;
using VideoApi.Domain;
using System.Linq;

namespace Video.API.Mappings
{
    public class ConferenceToDetailsResponseMapper
    {
        public ConferenceDetailsResponse MapConferenceToResponse(Conference conference,
            string pexipSelfTestNode)
        {
            var response = new ConferenceDetailsResponse
            {
                Id = conference.Id,
                HearingId = conference.HearingRefId,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                CurrentStatus = conference.GetCurrentStatus(),
                Participants =
                    new ParticipantToDetailsResponseMapper().MapParticipantsToResponse(conference.GetParticipants()),
                MeetingRoom = new MeetingRoomToResponseMapper().MapVirtualCourtToResponse(conference.GetMeetingRoom()),
                HearingVenueName = conference.HearingVenueName,
                Messages = conference.GetMessages().Select(x => new MessageToResponseMapper().MapMessageToResponse(x)).ToList()
            };

            if (response.MeetingRoom != null)
            {
                response.MeetingRoom.PexipSelfTestNode = pexipSelfTestNode;
            }
            
            return response;
        }
    }
}