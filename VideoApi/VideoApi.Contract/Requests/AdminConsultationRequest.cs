using System;
using System.ComponentModel.DataAnnotations;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// Request a private consultation with another participant
    /// </summary>
    public class AdminConsultationRequest
    {
        /// <summary>
        /// The conference UUID
        /// </summary>
        public Guid ConferenceId { get; set; }
        
        /// <summary>
        /// UUID of participant VH Officer attempted to call
        /// </summary>
        public Guid ParticipantId { get; set; }
        
        /// <summary>
        /// Consultation Room to use
        /// </summary>
        public RoomType ConsultationRoom { get; set; }
        
        /// <summary>
        /// Response to a consultation request (i.e. 'Accepted or Rejected')
        /// </summary>
        [EnumDataType(typeof(ConsultationAnswer))]
        public ConsultationAnswer? Answer { get; set; }
    }
}