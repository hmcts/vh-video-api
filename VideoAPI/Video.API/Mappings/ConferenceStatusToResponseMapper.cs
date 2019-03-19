using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class ConferenceStatusToResponseMapper
    {
        public ConferenceStatusResponse MapCurrentConferenceStatus(Conference conference)
        {
            var currentStatus = conference.GetCurrentStatus();
            if (currentStatus == null) return null;
            return new ConferenceStatusResponse
            {
                ConferenceState = currentStatus.ConferenceState,
                TimeStamp = currentStatus.TimeStamp
            };
        }
    }
}