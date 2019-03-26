using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Responses
{
    /// <summary>
    /// Detailed information for a conference
    /// </summary>
    public class ConferenceDetailsResponse
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseType { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public ConferenceStatusResponse CurrentStatus { get; set; }
        public List<ParticipantDetailsResponse> Participants { get; set; }
        public VirtualCourtResponse VirtualCourt { get; set; }
    }
}