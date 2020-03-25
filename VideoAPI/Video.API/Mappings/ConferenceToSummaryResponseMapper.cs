using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace Video.API.Mappings
{
    public static class ConferenceToSummaryResponseMapper
    {
        public static ConferenceSummaryResponse MapConferenceToSummaryResponse(Conference conference)
        {
            var participants = conference.GetParticipants().Select(x => ParticipantToSummaryResponseMapper.MapParticipantToSummary(x))
                .ToList();

            var activeTasks = conference.GetTasks().Where(x => x.Status == TaskStatus.ToDo).Select(x => TaskToResponseMapper.MapTaskToResponse(x)).ToList();

            return new ConferenceSummaryResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                Status = conference.GetCurrentStatus(),
                PendingTasks = conference.GetTasks().Count(x => x.Status == TaskStatus.ToDo),
                Participants = participants,
                HearingRefId = conference.HearingRefId,
                HearingVenueName = conference.HearingVenueName,
                Tasks = activeTasks
            };
        }
    }
}
