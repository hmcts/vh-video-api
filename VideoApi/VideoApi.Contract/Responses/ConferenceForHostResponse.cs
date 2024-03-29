using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    public class ConferenceForHostResponse
    {
        /// <summary>
        /// Conference UUID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Booking UUID
        /// </summary>
        public Guid HearingId { get; set; }

        /// <summary>
        /// Scheduled date time as UTC
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }
        
        /// <summary>
        /// The time a conference was closed
        /// </summary>
        public DateTime? ClosedDateTime { get; set; }

        /// <summary>
        /// The scheduled duration in minutes
        /// </summary>
        public int ScheduledDuration { get; set; }

        /// <summary>
        /// The case type
        /// </summary>
        public string CaseType { get; set; }

        /// <summary>
        /// The case number
        /// </summary>
        public string CaseNumber { get; set; }

        /// <summary>
        /// The case name
        /// </summary>
        public string CaseName { get; set; }

        /// <summary>
        /// The current conference status
        /// </summary>
        public ConferenceState Status { get; set; }

        /// <summary>
        /// The conference participants
        /// </summary>
        public List<ParticipantForHostResponse> Participants { get; set; }

        /// <summary>
        /// The number of video access endpoints for the hearing
        /// </summary>
        public int NumberOfEndpoints { get; set; }

        /// <summary>
        /// Flags true when hearing venue is in Scotland
        /// </summary>
        public bool HearingVenueIsScottish { get; set; }
    }

    public class ParticipantForHostResponse
    {
        /// <summary>
        /// The participant Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The participant display name during a conference
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The participant role in conference
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// The representee (if participant is a representative)
        /// </summary>
        public string Representee { get; set; }

        /// <summary>
        /// The group a participant belongs to
        /// </summary>
        public string CaseTypeGroup { get; set; }
        
        /// <summary>
        /// The participant hearing role in conference
        /// </summary>
        public string HearingRole { get; set; }
    }
}
