using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ConferenceToClosedConferenceMapper
    {
        public ClosedConferencesResponse MapConferenceToClosedResponse(Conference conference)
        {
            return new ClosedConferencesResponse
            {
                Id = conference.Id
            };
        }
    }
}
