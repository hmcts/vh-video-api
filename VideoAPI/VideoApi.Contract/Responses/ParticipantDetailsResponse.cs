using System;
using VideoApi.Domain.Enums;

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
        /// The representee (if participant is a representative)
        /// </summary>
        public string Representee { get; set; }
        
        /// <summary>
        /// The current participant status
        /// </summary>
        public ParticipantState CurrentStatus { get; set; }
        
        /// <summary>
        /// The self test results (if self-test completed)
        /// </summary>
        public TestCallScoreResponse SelfTestScore { get; set; }
    }
}
