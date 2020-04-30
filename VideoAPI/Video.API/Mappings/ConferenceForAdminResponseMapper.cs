using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public static class ConferenceForAdminResponseMapper
    {
        public static ConferenceForAdminResponse MapConferenceToSummaryResponse(Conference conference)
        {
            var participants = conference.GetParticipants().Select(ParticipantToSummaryResponseMapper.MapParticipantToSummary)
                .ToList();
            
            return new ConferenceForAdminResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                Status = conference.GetCurrentStatus(),
                Participants = participants,
                HearingRefId = conference.HearingRefId,
                HearingVenueName = conference.HearingVenueName,
            };
        }
    }
}
