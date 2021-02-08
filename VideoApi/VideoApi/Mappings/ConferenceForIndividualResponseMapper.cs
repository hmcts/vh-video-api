using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

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
                Status = conference.GetCurrentStatus().MapToContractEnum(),
                ClosedDateTime = conference.ClosedDateTime
            };
        }
    }
}
