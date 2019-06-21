using System;
using System.Collections.Generic;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    /// <summary>
    /// Detailed information for a conference
    /// </summary>
    public class ConferenceDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseType { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState CurrentStatus { get; set; }
        public List<ParticipantDetailsResponse> Participants { get; set; }
        public MeetingRoomResponse MeetingRoom { get; set; }
    }
}