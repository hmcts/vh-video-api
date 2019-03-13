using System;
using System.Collections.Generic;

namespace VideoApi.Contract.Responses
{
    public class ConferenceDetailsResponse
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseType { get; set; }
        public string CaseNumber { get; set; }
        public string AdminUri { get; set; }
        public string JudgeUri { get; set; }
        public string ParticipantUri { get; set; }
        public string PexipNode { get; set; }
        public ConferenceStatusResponse CurrentStatus { get; set; }
        public List<ParticipantDetailsResponse> Participants { get; set; }
    }
}