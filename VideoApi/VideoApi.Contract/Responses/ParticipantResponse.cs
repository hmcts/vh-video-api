using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantResponse
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
        /// The username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// The role of a user
        /// </summary>
        public UserRole UserRole { get; set; }
        
        /// <summary>
        /// The current participant status
        /// </summary>
        public ParticipantState CurrentStatus { get; set; }

        /// <summary>
        /// Current consultation room details
        /// </summary>
        public RoomResponse CurrentRoom { get; set; }
        
        /// <summary>
        /// Current interpreter room details
        /// </summary>
        public RoomResponse CurrentInterpreterRoom { get; set; }

        /// <summary>
        /// Linked Participants - example interpreter and "interpretee"
        /// </summary>
        public IList<LinkedParticipantResponse> LinkedParticipants { get; set; }
    }
}
