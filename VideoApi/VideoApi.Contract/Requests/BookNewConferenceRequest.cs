using System;
using System.Collections.Generic;
using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests
{
    public class BookNewConferenceRequest
    {
        public BookNewConferenceRequest()
        {
            Endpoints = new List<AddEndpointRequest>();   
        }

        public string CaseTypeServiceId { get; set; }
        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
        public string HearingVenueName { get; set; }
        public bool AudioRecordingRequired { get; set; }
        public List<AddEndpointRequest> Endpoints { get; set; }
        public Supplier Supplier { get; set; }
        public ConferenceRoomType ConferenceRoomType { get; set; }
    }
}
