namespace VideoApi.Contract.Requests
{
    public class AddEndpointRequest
    {
        /// <summary>
        /// The display name for an endpoint
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The sip address to connect via
        /// </summary>
        public string SipAddress { get; set; }
        
        /// <summary>
        /// The pin
        /// </summary>
        public string Pin { get; set; }
    }
}
