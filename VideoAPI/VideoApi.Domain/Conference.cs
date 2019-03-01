using System;
using System.Collections.Generic;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Conference : Entity<Guid>
    {
        public Conference(Guid hearingRefId, string caseType, DateTime scheduledDateTime, string caseNumber)
        {
            Id = Guid.NewGuid();
            Participants = new List<Participant>();
            
            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
        }
        
        public Guid HearingRefId { get; protected set; }
        public string CaseType { get; protected set; }
        public DateTime ScheduledDateTime { get; protected set; }
        public string CaseNumber { get; protected set; }
        protected virtual IList<Participant> Participants { get; set; }
        protected virtual IList<ConferenceStatus> StatusHistory { get; set; }

        public IList<Participant> GetParticipants()
        {
            return Participants;
        }

        public void UpdateConferenceStatus(ConferenceState status)
        {
            StatusHistory.Add(new ConferenceStatus(status));
        }
        
        public IList<ConferenceStatus> GetStatusHistory()
        {
            return StatusHistory;
        }
    }
}