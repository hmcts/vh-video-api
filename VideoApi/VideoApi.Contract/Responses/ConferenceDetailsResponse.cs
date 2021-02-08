using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses
{
    /// <summary>
    /// Detailed information for a conference
    /// </summary>
    public class ConferenceDetailsResponse
    {
        /// <summary>
        /// The conference's UUID
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The UUID of the booking
        /// </summary>
        public Guid HearingId { get; set; }
        
        /// <summary>
        /// The scheduled start time of a conference
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }
        
        /// <summary>
        /// The time a conference started (not resumed)
        /// </summary>
        public DateTime? StartedDateTime { get; set; }
        
        /// <summary>
        /// The time a conference was closed
        /// </summary>
        public DateTime? ClosedDateTime { get; set; }
        
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
        /// The scheduled duration of a conference in minutes
        /// </summary>
        public int ScheduledDuration { get; set; }
        
        /// <summary>
        /// The current conference status
        /// </summary>
        public ConferenceState CurrentStatus { get; set; }
        
        /// <summary>
        /// List of participants in conference
        /// </summary>
        public List<ParticipantDetailsResponse> Participants { get; set; }
        
        /// <summary>
        /// List of endpoints in conference
        /// </summary>
        public List<EndpointResponse> Endpoints { get; set; }
        
        /// <summary>
        /// The Kinly meeting room details
        /// </summary>
        public MeetingRoomResponse MeetingRoom { get; set; }
        
        public string HearingVenueName { get; set;  }

        /// <summary>
        /// The option to indicate hearing audio recording
        /// </summary>
        public bool AudioRecordingRequired { get; set; }
    }
}
