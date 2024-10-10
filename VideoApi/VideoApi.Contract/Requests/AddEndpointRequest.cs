using VideoApi.Contract.Enums;

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
        
        /// <summary>
        /// Username of a defence advocate
        /// </summary>
        public string DefenceAdvocate { get; set; }

        /// <summary>
        /// Role of the endpoint
        /// </summary>
        public Role Role { get; set; }
    }
}
