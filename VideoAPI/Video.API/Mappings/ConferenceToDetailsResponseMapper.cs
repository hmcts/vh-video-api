using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ConferenceToDetailsResponseMapper
    {
        public ConferenceDetailsResponse MapConferenceToResponse(Conference conference)
        {
            var response = new ConferenceDetailsResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                CurrentStatus = new ConferenceStatusToResponseMapper().MapCurrentConferenceStatus(conference),
                Participants =
                    new ParticipantToDetailsResponseMapper().MapParticipantsToResponse(conference.GetParticipants()),
                VirtualCourt = new VirtualCourtToResponseMapper().MapVirtualCourtToResponse(conference.GetVirtualCourt())
            };
            return response;
        }
    }
}