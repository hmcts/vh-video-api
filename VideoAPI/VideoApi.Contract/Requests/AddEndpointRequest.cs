namespace VideoApi.Contract.Requests
{
    public class AddEndpointRequest
    {
        /// <summary>
        /// The endpoint display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The endpoint sip address
        /// </summary>
        public string SipAddress { get; set; }
        
        /// <summary>
        /// The endpoint pin
        /// </summary>
        public string Pin { get; set; }
    }
}
