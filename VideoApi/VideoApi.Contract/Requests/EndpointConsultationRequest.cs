using System;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// Request a private consultation with a video endpoint
    /// </summary>
    public class EndpointConsultationRequest
    {
        /// <summary>
        /// The conference UUID
        /// </summary>
        public Guid ConferenceId { get; set; }
        
        /// <summary>
        /// Defence Advocate's UUID
        /// </summary>
        public Guid DefenceAdvocateId { get; set; }
        
        /// <summary>
        /// Video Endpoint's UUID
        /// </summary>
        public Guid EndpointId { get; set; } 
    }
}
