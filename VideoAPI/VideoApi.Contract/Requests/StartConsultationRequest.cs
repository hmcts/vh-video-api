using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    /// <summary>
    /// Start a private consultation
    /// </summary>
    public class StartConsultationRequest
    {
        /// <summary>
        /// The conference UUID
        /// </summary>
        public Guid ConferenceId { get; set; }
        
        /// <summary>
        /// The room type number
        /// </summary>
        public RoomType RoomType { get; set; }
        
        /// <summary>
        /// Requester's UUID
        /// </summary>
        public Guid RequestedBy { get; set; }
    }
}
