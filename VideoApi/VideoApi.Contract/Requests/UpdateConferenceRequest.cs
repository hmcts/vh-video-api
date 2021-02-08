using System;

namespace VideoApi.Contract.Requests
{
    public class UpdateConferenceRequest
    {
        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public string HearingVenueName { get; set; }
        public bool AudioRecordingRequired { get; set; }
    }
}
