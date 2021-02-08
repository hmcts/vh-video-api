using System;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class JudgeInHearingResponse
    {
        /// <summary>
        /// The participant id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The conference id
        /// </summary>
        public Guid ConferenceId { get; set; }
        
        /// <summary>
        /// The participant username
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The current participant status
        /// </summary>
        public ParticipantState Status { get; set; }
        
        /// <summary>
        /// The participant role in conference
        /// </summary>
        public UserRole UserRole { get; set; }
        
        /// <summary>
        /// The group a participant belongs to
        /// </summary>
        public string CaseGroup { get; set; }
    }
}
