using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ConferenceToDetailsResponseMapper
    {
        public static ConferenceDetailsResponse MapConferenceToResponse(Conference conference,
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
                StartedDateTime = conference.ActualStartTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                CurrentStatus = conference.GetCurrentStatus().MapToContractEnum(),
                Participants =
                    ParticipantToDetailsResponseMapper.MapParticipantsToResponse(conference.GetParticipants()),
                MeetingRoom = MeetingRoomToResponseMapper.MapVirtualCourtToResponse(conference.GetMeetingRoom()),
                Endpoints = conference.GetEndpoints().Select(EndpointToResponseMapper.MapEndpointResponse).ToList(),
                HearingVenueName = conference.HearingVenueName,
                AudioRecordingRequired = conference.AudioRecordingRequired,
            };

            if (response.MeetingRoom != null)
            {
                response.MeetingRoom.PexipSelfTestNode = pexipSelfTestNode;
            }
            
            return response;
        }
    }
}
