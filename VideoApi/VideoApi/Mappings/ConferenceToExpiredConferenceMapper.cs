using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ConferenceToExpiredConferenceMapper
    {
        public static ExpiredConferencesResponse MapConferenceToExpiredResponse(Conference conference)
        {
            return new ExpiredConferencesResponse
            {
                Id = conference.Id,
                CurrentStatus = conference.State,
                HearingId = conference.HearingRefId
            };
        }
    }
}
