using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class UpdateEndpointRequest
    {
        public string DisplayName { get; set; }
        public ConferenceRole ConferenceRole { get; set; }
    }
}
