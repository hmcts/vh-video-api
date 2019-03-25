using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Video.API.Mappings
{
    public class ConferenceToSummaryResponseMapper
    {
        public ConferenceSummaryResponse MapConferenceToSummaryResponse(Conference conference)
        {
            var conferenceStatus = conference.GetCurrentStatus() != null
                ? conference.GetCurrentStatus().ConferenceState
                : ConferenceState.None;

            var participantMapper = new ParticipantToSummaryResponseMapper();
            var participants = conference.GetParticipants().Select(x => participantMapper.MapParticipantToSummary(x))
                .ToList();
            
            return new ConferenceSummaryResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                ScheduledDuration= conference.ScheduledDuration,
                Status = conferenceStatus,
                Participants = participants
            };
        }       
    }
}