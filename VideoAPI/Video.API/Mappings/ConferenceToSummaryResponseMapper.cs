using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ConferenceToSummaryResponseMapper
    {
        public ConferenceSummaryResponse MapConferenceToSummaryResponse(Conference conference)
        {
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
                Status = conference.GetCurrentStatus(),
                Participants = participants
            };
        }       
    }
}