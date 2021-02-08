using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ConferenceToExpiredConferenceMapper
    {
        public static ExpiredConferencesResponse MapConferenceToExpiredResponse(Conference conference)
        {
            return new ExpiredConferencesResponse
            {
                Id = conference.Id,
                CurrentStatus = conference.State.MapToContractEnum(),
                HearingId = conference.HearingRefId
            };
        }
    }
}
