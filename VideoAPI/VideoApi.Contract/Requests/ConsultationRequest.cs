using System;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// Request a private consultation with another participant
    /// </summary>
    public class ConsultationRequest
    {
        /// <summary>
        /// The virtual courtoom UUID
        /// </summary>
        public Guid ConferenceId { get; set; }
        
        /// <summary>
        /// Requestor's UUID
        /// </summary>
        public Guid RequestedBy { get; set; }
        
        /// <summary>
        /// Requestee's UUID
        /// </summary>
        public Guid RequestedFor { get; set; }
    }
}