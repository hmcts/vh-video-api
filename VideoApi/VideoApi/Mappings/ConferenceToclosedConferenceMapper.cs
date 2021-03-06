using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ConferenceToClosedConferenceMapper
    {
        public static ClosedConferencesResponse MapConferenceToClosedResponse(Conference conference)
        {
            return new ClosedConferencesResponse
            {
                Id = conference.Id
            };
        }
    }
}
