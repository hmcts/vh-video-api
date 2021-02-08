using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantSummaryResponse
    {
        /// <summary>
        /// The participant id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The participant username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The participant display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The participant first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The participant last name
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
        /// The current participant status
        /// </summary>
        public ParticipantState Status { get; set; }
        
        /// <summary>
        /// The participant role in conference
        /// </summary>
        public UserRole UserRole { get; set; }
        
        /// <summary>
        /// The participant hearing role in conference
        /// </summary>
        public string HearingRole { get; set; }
        
        /// <summary>
        /// The representee (if participant is a representative)
        /// </summary>
        public string Representee { get; set; }
        
        /// <summary>
        /// The group a participant belongs to
        /// </summary>
        public string CaseGroup { get; set; }
    }
}
