using System;
using System.ComponentModel.DataAnnotations;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// Request a private consultation with another participant
    /// </summary>
    public class ConsultationRequest
    {
        /// <summary>
        /// The conference UUID
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
        
        /// <summary>
        /// Response to a consultation request (i.e. 'Accepted or Rejected')
        /// </summary>
        [EnumDataType(typeof(ConsultationAnswer))]
        public ConsultationAnswer? Answer { get; set; }
    }
    
    public enum ConsultationAnswer
    {
        /// <summary>
        /// Default when no answer has been provided
        /// </summary>
        None,
        /// <summary>
        /// Accept a consultation request
        /// </summary>
        Accepted,
        /// <summary>
        /// Reject a consultation request
        /// </summary>
        Rejected
    }
}