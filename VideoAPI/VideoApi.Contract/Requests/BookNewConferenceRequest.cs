using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Requests
{
    public class BookNewConferenceRequest
    {
        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
    }
}