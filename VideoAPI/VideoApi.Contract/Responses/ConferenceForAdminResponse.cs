using System;
using System.Collections.Generic;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ConferenceForAdminResponse
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? StartedDateTime { get; set; }
        public DateTime? ClosedDateTime { get; set; }
        public string CaseType { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState Status { get; set; }
        public List<ParticipantSummaryResponse> Participants { get; set; }
        public Guid HearingRefId { get; set; }
        public string HearingVenueName { get; set; }
        public string TelephoneConferenceId { get; set; }
    }
}
