using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ConferenceSummaryResponse
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseType { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState Status { get; set; }
        public ParticipantSummary Participants { get; set; }

    }
}