using System;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// Request a private consultation with another participant
    /// </summary>
    public class ConsultationRequestResponse
    {
        /// <summary>
        /// The conference UUID
        /// </summary>
        public Guid ConferenceId { get; set; }
        
        /// <summary>
        /// Requestor's UUID
        /// This value can be empty is the consultation is requested by a VHO
        /// </summary>
        public Guid RequestedBy { get; set; }
        
        /// <summary>
        /// Requestee's UUID
        /// </summary>
        public Guid RequestedFor { get; set; }

        /// <summary>
        /// The room to have a private consultation in
        /// </summary>
        public string RoomLabel { get; set; }
        
        /// <summary>
        /// Response to a consultation request (i.e. 'Accepted or Rejected')
        /// </summary>
        public ConsultationAnswer Answer { get; set; }
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
