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
        /// Defence Advocate or Judge or JOH's UUID
        /// </summary>
        public Guid RequestedById { get; set; }
        
        /// <summary>
        /// Video Endpoint's UUID
        /// </summary>
        public Guid EndpointId { get; set; }

        /// <summary>
        /// The label / name of the room to lock/unlock
        /// </summary>
        public string RoomLabel { get; set; }
    }
}
