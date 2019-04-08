using System;
using System.ComponentModel.DataAnnotations;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// This is used to answer a request a private consultation with another participant
    /// </summary>
    public class ConsultationResultRequest
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
        [EnumDataType(typeof(ConsultationRequestAnswer))]
        public ConsultationRequestAnswer Answer { get; set; }
    }
}