using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class UpdateParticipantRequest
    {
        public UpdateParticipantRequest()
        {
            LinkedParticipants = new List<LinkedParticipantRequest>();
        }

        /// <summary>
        ///     Participant Ref Id
        /// </summary>
        public Guid ParticipantRefId { get; set; }

        /// <summary>
        ///     Participant Fullname
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        ///     Participant FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Participant LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Participant Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Representee
        /// </summary>
        public string Representee { get; set; }

        /// <summary>
        /// The participant contact email
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// The participant contact telephone
        /// </summary>
        public string ContactTelephone { get; set; }
        
        /// <summary>
        /// The participant username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Linked participants
        /// </summary>
        public IList<LinkedParticipantRequest> LinkedParticipants { get; set; }
        
        /// <summary>
        /// The participant user role
        /// </summary>
        public UserRole UserRole { get; set; }
        
        /// <summary>
        /// The participant hearing role
        /// </summary>
        public string HearingRole { get; set; }
    }
}
