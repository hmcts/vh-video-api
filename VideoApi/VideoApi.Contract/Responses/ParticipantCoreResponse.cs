using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantCoreResponse
    {
        /// <summary>
        /// The participant's UUID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The UUID for a participant within Bookings
        /// </summary>
        public Guid RefId { get; set; }
        
        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The role of a user
        /// </summary>
        public UserRole UserRole { get; set; }
    }
}
