using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ConferenceForIndividualResponseMapper
    {
        public static ConferenceForIndividualResponse MapConferenceSummaryToModel(Conference conference)
        {
            return new ConferenceForIndividualResponse
            {
                Id = conference.Id,
                CaseName = conference.CaseName,
                CaseNumber = conference.CaseNumber,
                ScheduledDateTime = conference.ScheduledDateTime,
                Status = conference.State,
                ClosedDateTime = conference.ClosedDateTime
            };
        }
    }
}
