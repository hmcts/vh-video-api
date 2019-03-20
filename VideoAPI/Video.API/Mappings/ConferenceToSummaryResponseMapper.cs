using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ConferenceToSummaryResponseMapper
    {
        public ConferenceSummaryResponse MapConferenceToSummaryResponse(Conference conference)
        {
            return new ConferenceSummaryResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime
            };
        }
    }
}