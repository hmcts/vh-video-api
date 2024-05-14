namespace VideoApi.Contract.Requests
{
    public class UpdateEndpointRequest
    {
        public string DisplayName { get; set; }
        public EndpointParticipantRequest[] EndpointParticipants { get; set; }
    }
}
