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
            var participantMapper = new ParticipantToSummaryResponseMapper();
            var participants = conference.GetParticipants().Select(x => participantMapper.MapParticipantToSummary(x))
                .ToList();
            
            return new ConferenceSummaryResponse
            {
                Id = conference.Id,
                HearingId = conference.HearingRefId,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration= conference.ScheduledDuration,
                Status = conference.GetCurrentStatus(),
                PendingTasks = conference.GetTasks().Count(x => x.Status == TaskStatus.ToDo),
                Participants = participants
            };
        }       
    }
}