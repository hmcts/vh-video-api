using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class EndpointResponse
    {
        /// <summary>
        /// The endpoint id
        /// </summary>
        public Guid Id { get; set; }
        
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
        
        /// <summary>
        /// The current endpoint status
        /// </summary>
        public EndpointState Status { get; set; }
        
        /// <summary>
        /// The defence advocate
        /// </summary>
        public string DefenceAdvocate { get; set; }

        /// <summary>
        /// Current consultation room details
        /// </summary>
        public RoomResponse CurrentRoom { get; set; }

        /// <summary>
        /// The role in the conference
        /// </summary>
        public ConferenceRole ConferenceRole { get; set; }
    }
}
