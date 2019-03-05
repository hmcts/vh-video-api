using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    public class UpdateConferenceStatusRequest
    {
        public ConferenceState State { get; set; }
    }
}