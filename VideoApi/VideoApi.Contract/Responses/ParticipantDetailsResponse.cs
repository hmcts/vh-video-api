using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantDetailsResponse
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
        /// The full name of a participant
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The first name of a participant
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of a participant
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// The participant contact email
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// The participant contact telephone
        /// </summary>
        public string ContactTelephone { get; set; }

        /// <summary>
        /// The display name
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// The username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The role of a user
        /// </summary>
        public UserRole UserRole { get; set; }
        
        /// <summary>
        /// The group participant belongs to (e.g. Claimant of Defendant)
        /// </summary>
        public string CaseTypeGroup { get; set; }
        
        /// <summary>
        /// The hearing role for a participant
        /// </summary>
        public string HearingRole { get; set; }
        
        /// <summary>
        /// The representee (if participant is a representative)
        /// </summary>
        public string Representee { get; set; }
        
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
