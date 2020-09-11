namespace VideoApi.Contract.Requests
{
    public class AddEndpointRequest
    {
        public string DisplayName { get; set; }
        public string SipAddress { get; set; }
        public string Pin { get; set; }
        public string DefenceAdvocate { get; set; }
    }
}
