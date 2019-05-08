using System;
using System.Collections.Generic;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Requests
{
    public class BookNewConferenceRequest
    {
        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public virtual MeetingRoom MeetingRoom { get; set; }
        public int ScheduledDuration { get; set; }
        public ConferenceState State { get; set; }
        public List<ParticipantRequest> Participants { get; set; }
        public virtual IList<ConferenceStatus> ConferenceStatuses { get; set; }
        public virtual IList<Task> Tasks { get; set; }
    }
}