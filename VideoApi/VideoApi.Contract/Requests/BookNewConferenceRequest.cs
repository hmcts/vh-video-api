using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class BookNewConferenceRequest
    {
        public BookNewConferenceRequest()
        {
            Endpoints = new List<AddEndpointRequest>();   
        }
        
        /// <summary>
        /// The booking reference ID
        /// </summary>
        public Guid HearingRefId { get; set; }
        
        /// <summary>
        /// The ID of the group of hearings
        /// </summary>
        public Guid? GroupId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
        public string HearingVenueName { get; set; }
        public bool AudioRecordingRequired { get; set; }
        public List<AddEndpointRequest> Endpoints { get; set; }
    }
}
