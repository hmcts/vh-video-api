using System;
using System.Collections.Generic;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public class Participant : Entity<long>
    {
        public Participant(Guid participantRefId, string name, string displayName, string username, string hearingRole,
            string caseTypeGroup)
        {
            ParticipantRefId = participantRefId;
            DisplayName = displayName;
            Username = username;
            HearingRole = hearingRole;
            CaseTypeGroup = caseTypeGroup;
            Name = name;
        }

        public Guid ParticipantRefId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string HearingRole { get; set; }
        public string CaseTypeGroup { get; set; }
        protected virtual IList<ParticipantStatus> StatusHistory { get; set; }

        public void UpdateParticipantStatus(ParticipantState status)
        {
            StatusHistory.Add(new ParticipantStatus(status));
        }
    }

    public class ParticipantStatus : Entity<long>
    {
        public ParticipantStatus(ParticipantState participantState)
        {
            ParticipantState = participantState;
            TimeStamp = DateTime.UtcNow;
        }

        public ParticipantState ParticipantState { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}