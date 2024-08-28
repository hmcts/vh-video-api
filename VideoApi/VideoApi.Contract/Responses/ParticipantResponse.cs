using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantResponse : ParticipantCoreResponse
    {
        /// <summary>
        /// The username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The contact email
        /// </summary>
        public string ContactEmail { get; set; }
        
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
