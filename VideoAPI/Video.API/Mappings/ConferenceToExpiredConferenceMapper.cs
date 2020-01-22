using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ConferenceToExpiredConferenceMapper
    {
        public ExpiredConferencesResponse MapConferenceToExpiredResponse(Conference conference)
        {
            return new ExpiredConferencesResponse
            {
                Id = conference.Id,
                CurrentStatus = conference.State
            };
        }
    }
}